using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StreamSimulator.Core.Extensions;

namespace StreamSimulator.Core
{
    public class ClientInfo
    {
        #region // Events
        public delegate void SubscriptionReceived(string symbol);
        public event SubscriptionReceived OnSubscriptionReceived;

        public delegate void UnsubscribeReceived(string symbol);
        public event UnsubscribeReceived OnUnsubscribeReceived;

        public delegate void MessageReceived(string message);
        public event MessageReceived OnMessageReceived;

        public delegate void Disconnect(Guid clientId);
        public event Disconnect OnDisconnect;
        #endregion

        private readonly TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private Task _listeningTask;
        private CancellationTokenSource _cancellationToken;

        private bool _keepListening = true;
        private bool _keepSending = true;
        private int _sleepTimer = 2500;
        private bool _sendNaNValues = false;
        private List<string> _symbols = new List<string>();


        public Guid ClientId { get; }
        private StreamSettings _settings;

        public ClientInfo(TcpClient tcpClient, StreamSettings settings)
        {
            ClientId = Guid.NewGuid();
            _tcpClient = tcpClient;
            _settings = settings;

            StartListening();
        }

        public void SendNaNValues(bool flag)
        {
            _sendNaNValues = flag;
        }

        public void StartListening()
        {
            _cancellationToken = new CancellationTokenSource();
            _networkStream = _tcpClient.GetStream();

            _listeningTask = new Task(() =>
            {
                $"### Client {ClientId} starts listening now...".Dump();

                try
                {
                    while (_networkStream != null && _networkStream.CanRead && !_cancellationToken.Token.IsCancellationRequested)
                    {
                        byte[] rcvBuffer = new byte[_tcpClient.ReceiveBufferSize];
                        int sumNumberOfBytesRead = 0;
                        int numberOfBytesRead = 0;
                        string completeMessage = string.Empty;

                        //do
                        //{
                        //    numberOfBytesRead = _networkStream.Read(rcvBuffer, 0, rcvBuffer.Length);
                        //    sumNumberOfBytesRead += numberOfBytesRead;
                        //    completeMessage = string.Concat(completeMessage, Encoding.ASCII.GetString(rcvBuffer, 0, numberOfBytesRead));
                        //}
                        //while (_networkStream.DataAvailable);

                        numberOfBytesRead = _networkStream.Read(rcvBuffer, 0, rcvBuffer.Length);
                        sumNumberOfBytesRead += numberOfBytesRead;
                        completeMessage = string.Concat(completeMessage, Encoding.ASCII.GetString(rcvBuffer, 0, numberOfBytesRead));

                        if (!string.IsNullOrWhiteSpace(completeMessage))
                        {
                            $" RECEIVING | {completeMessage}".Dump();

                            var singleCommands = completeMessage.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var singleCommand in singleCommands)
                            {
                                foreach (var subscribeCommand in _settings.SubscribeCommand.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (singleCommand.StartsWith(subscribeCommand))
                                    {
                                        var symbol = singleCommand.Substring(singleCommand.IndexOf(" ")).Trim();
                                        _symbols.Add(symbol);
                                        OnSubscriptionReceived?.Invoke(symbol);
                                    }
                                }

                                foreach (var unsubscribeCommand in _settings.UnsubscribeCommand.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (singleCommand.StartsWith(unsubscribeCommand))
                                    {
                                        var symbol = singleCommand.Substring(singleCommand.IndexOf(" ")).Trim();
                                        _symbols.Remove(symbol);
                                        OnUnsubscribeReceived?.Invoke(symbol);
                                    }
                                }
                            }

                            OnMessageReceived?.Invoke(completeMessage);
                        }

                        if (_settings.SendReplyCommands)
                        {
                            foreach (var key in _settings.ReplyCommands.Keys)
                            {
                                if (completeMessage.Contains(key))
                                {
                                    SendMessage("MESSAGE", _settings.ReplyCommands[key]);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    $"### Client {ClientId} throws exception: {ex.Message}".Dump();
                }

                $"### Client {ClientId} stops listening now...".Dump();
                this.Close();
            }, _cancellationToken.Token);
            _listeningTask.Start();
        }

        public void SendMessage(string symbol, string input)
        {
            if (_networkStream != null && _networkStream.CanWrite)
            {
                if (string.Compare("MESSAGE", symbol, true) == 0 || _symbols.Contains(symbol))
                {
                    var data = ConvertData(input);
                    _networkStream.Write(data, 0, data.Length);

                    $" SENDING | {input}".Dump();
                }
            }
        }

        public string[] GetSymbols()
        {
            return _symbols?.ToArray() ?? new string[0];
        }

        public void Close()
        {
            _cancellationToken.Cancel();

            if (_networkStream != null)
            {
                _networkStream.Close();
                _networkStream.Dispose();
            }

            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient.Dispose();
            }

            $"### Client {ClientId} was terminated...".Dump();

            OnDisconnect?.Invoke(ClientId);
        }

        public void SetSleepTimer(int sleepTime)
        {
            $"### Client {ClientId} adjusts sleepTimer to {sleepTime}ms...".Dump();
            _sleepTimer = sleepTime;
        }

        private byte[] ConvertData(string data, bool suppressEndline = false)
        {
            if (!data.EndsWith("\n") && suppressEndline == false)
            {
                data += "\n";
            }

            return Encoding.ASCII.GetBytes(data);
        }

        public override string ToString()
        {
            return $"Client {ClientId} | listening? {_keepListening} | sending? {_keepSending} | message interval: {_sleepTimer}ms";
        }
    }
}
