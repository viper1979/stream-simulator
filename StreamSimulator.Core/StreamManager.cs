using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StreamSimulator.Core
{
    public class StreamManager
    {
        private Dictionary<int, StreamingServer> _streamSimulators;

        public StreamManager()
        {
            _streamSimulators = new Dictionary<int, StreamingServer>();
        }

        public void AddSimulator(StreamSettings settings)
        {
            if (this._streamSimulators.ContainsKey(settings.ListeningPort))
            {
                throw new ArgumentException("port is already assigned.");
            }

            var simulator = StreamingServer.CreateServer(settings);
            simulator.Start();

            _streamSimulators.Add(simulator.ListeningPort, simulator);
        }

        public bool RemoveSimulator(Guid identifier)
        {
            var simulator = _streamSimulators.FirstOrDefault(item => item.Value.Identifier == identifier).Value;
            if (simulator != null)
            {
                simulator.Stop();
                _streamSimulators.Remove(simulator.ListeningPort);
                return true;
            }

            return false;
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
