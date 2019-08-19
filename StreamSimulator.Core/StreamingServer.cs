using Fare;
using StreamSimulator.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamSimulator.Core
{
    public class StreamingServer
    {
        public delegate void MessageUpdate(string symbol, string message);
        public event MessageUpdate OnMessageUpdate;

        private readonly TcpListener _listener;
        private Task _generateUpdatesTask;
        private Task _generateHeartbeatTask;
        public ConcurrentBag<ClientInfo> Clients = new ConcurrentBag<ClientInfo>();
        private CancellationTokenSource _cancellationTokenSource;
        private readonly StreamSettings _streamSettings;
        private readonly Xeger _xeger;
        private bool _keepListening = true;
        private StringBuilder _batchStringBuilder = new StringBuilder();

        private bool _forceSendUpdate = true;
        private bool _silentMode;
        private readonly Dictionary<string, int> _symbols;

        public static StreamingServer CreateServer(StreamSettings settings)
        {
            return new StreamingServer(IPAddress.Any, settings);
        }

        public StreamingServer(IPAddress ipAddress, StreamSettings settings)
        {
            Identifier = Guid.NewGuid();
            _streamSettings = settings;
            _symbols = new Dictionary<string, int>();
            _xeger = new Xeger(_streamSettings.MessagePattern, new Random(DateTime.Now.Millisecond));

            _listener = new TcpListener(ipAddress, _streamSettings.ListeningPort);
        }

        public void Start()
        {
            _keepListening = true;
            _listener.Start();

            $"### Server started...".Dump();

            Initialize();

            AcceptConnections();
        }

        public void Stop()
        {
            if (Clients != null && Clients.Count > 0)
            {
                foreach (var client in Clients)
                {
                    client.Close();
                }
            }
        }

        private async void AcceptConnections()
        {
            while (_keepListening)
            {
                $"### Waiting for incoming requests...".Dump();
                var tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                var client = new ClientInfo(tcpClient, _streamSettings);

                tcpClient.SendTimeout = 1000;
                tcpClient.LingerState = new LingerOption(true, 10);

                client.OnSubscriptionReceived += OnSubscriptionReceivedHandler;
                client.OnUnsubscribeReceived += OnUnsubscribeReceived;
                client.OnDisconnect += OnDisconnectHandler;
                OnMessageUpdate += client.SendMessage;

                Clients.Add(client);
            }
        }

        private void OnDisconnectHandler(Guid clientId)
        {
            var client = Clients.FirstOrDefault(c => c.ClientId == clientId);
            if (client != null)
            {
                $"### Client {clientId} disconnected...".Dump();
                client.OnSubscriptionReceived -= OnSubscriptionReceivedHandler;
                client.OnUnsubscribeReceived -= OnUnsubscribeReceived;
                client.OnDisconnect -= OnDisconnectHandler;
                OnMessageUpdate -= client.SendMessage;

                var symbols = client.GetSymbols();
                foreach (var symbol in symbols)
                {
                    OnUnsubscribeReceived(symbol);
                }

                Clients.TryTake(out client);
            }
            else
            {
                $"### No client with id {clientId} found...".Dump();
            }

            if (Clients.Count == 0 && _symbols.Any())
            {
                _symbols.Clear();
            }
        }

        private void OnSubscriptionReceivedHandler(string symbol)
        {
            if (_symbols.ContainsKey(symbol))
            {
                _symbols[symbol] += 1;
            }
            else
            {
                _symbols.Add(symbol, 1);
            }
        }

        private void OnUnsubscribeReceived(string symbol)
        {
            if (_symbols.ContainsKey(symbol))
            {
                int count = _symbols[symbol];
                if (count == 1)
                {
                    _symbols.Remove(symbol);
                }
                else
                {
                    _symbols[symbol] -= 1;
                }
            }
        }

        private void Initialize()
        {
            if (_cancellationTokenSource == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            _generateUpdatesTask = new Task(this.SendQuoteUpdate, _cancellationTokenSource.Token);
            _generateUpdatesTask.Start();

            if (_streamSettings.SendHeartbeat)
            {
                _generateHeartbeatTask = new Task(this.SendHeartbeatUpdate, _cancellationTokenSource.Token);
                _generateHeartbeatTask.Start();
            }
        }

        /***/

        private void SendQuoteUpdate()
        {
            while (_cancellationTokenSource.IsCancellationRequested == false)
            {
                if (Clients.Count == 0 || (_symbols.Count == 0 && !_forceSendUpdate))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                var startTime = DateTime.Now;

                SendMessages(_streamSettings.SimultaneousMessageCount, _streamSettings.MessagesPerSecond);

                var consumedTime = DateTime.Now.Subtract(startTime);
                if (consumedTime.TotalMilliseconds < 1000)
                {
                    Thread.Sleep(1000 - (int) consumedTime.TotalMilliseconds);
                }

                $"Messages Per Second: {_streamSettings.MessagesPerSecond / consumedTime.TotalSeconds}".Dump();
            }
        }

        private void SendMessages(int batchSize, int messagesPerSecond)
        {
            var symbol = _streamSettings.Symbols.FirstOrDefault() ?? "DummySymbol";
            // 'MESSAGE' just sends the message to the client without filtering for a symbol
            if (!SilentMode)
            {
                for (int i = 0; i < messagesPerSecond; i++)
                {
                    _batchStringBuilder.Clear();

                    for (int j = 0; j < batchSize; j++)
                    {
                        int lineCount = 0;
                        int index = 0;

                        while (lineCount < _symbols.Count)
                        {
                            if (index >= _symbols.Count)
                            {
                                index = 0;
                            }

                            _batchStringBuilder.AppendLine($"{_symbols.ElementAt(index).Key};{_xeger.Generate()}");
                        }
                    }

                    OnMessageUpdate?.Invoke("MESSAGE", symbol + ";" + _xeger.Generate());
                }



                for (int i = 0; i < _streamSettings.SimultaneousMessageCount; i++)
                {
                    
                }
            }
        }

        private void SendHeartbeatUpdate()
        {
            while (_cancellationTokenSource.IsCancellationRequested == false)
            {
                if (!SilentMode)
                {
                    OnMessageUpdate?.Invoke("MESSAGE", _streamSettings.HeartbeatPattern);
                }
                Thread.Sleep(_streamSettings.HeartbeatIntervalMs);
            }
        }

        /***/

        #region //  Properties

        public Guid Identifier { get; }

        public int ListeningPort => _streamSettings.ListeningPort;

        public bool SilentMode
        {
            get { return _silentMode; }
            set { $"### SilentMode set to {value}".Dump(); _silentMode = value; }
        }

        #endregion
    }
}
