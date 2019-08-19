using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamSimulator.Core
{
    public class StreamManager
    {
        private readonly Dictionary<int, StreamingServer> _streamSimulators;

        public StreamManager()
        {
            _streamSimulators = new Dictionary<int, StreamingServer>();
        }

        public StreamingServer Add(StreamSettings settings)
        {
            if (_streamSimulators.ContainsKey(settings.ListeningPort))
            {
                throw new ArgumentException("port is already assigned.");
            }

            var simulator = StreamingServer.CreateServer(settings);
            simulator.Start();

            _streamSimulators.Add(simulator.ListeningPort, simulator);
            return simulator;
        }

        public bool Stop(int port)
        {
            if (!_streamSimulators.ContainsKey(port)) return false;

            _streamSimulators[port].Stop();
            return true;
        }

        public bool Start(int port)
        {
            if (!_streamSimulators.ContainsKey(port)) return false;

            _streamSimulators[port].Start();
            return true;
        }

        public bool Remove(int port)
        {
            if (!_streamSimulators.ContainsKey(port)) return false;

            _streamSimulators[port].Stop();
            _streamSimulators.Remove(port);
            return true;
        }

        public string[] GetSimulators()
        {
            return _streamSimulators.Select(item =>
            {
                return "Guid: " + item.Value.Identifier.ToString() + 
                       " Port: " + item.Value.ListeningPort +
                       " Clients: " + item.Value.Clients.Count;
            }).ToArray();
        }
    }
}
