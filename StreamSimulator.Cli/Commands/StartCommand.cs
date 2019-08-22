using StreamSimulator.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamSimulator.Cli
{
    public class StartCommand : CliCommand
    {
        public StreamSettings Settings { get; }

        public StartCommand()
        {
            Type = CommandType.Start;
            Key = "start";

            Settings = new StreamSettings
            {
                ListeningPort = 9999
            };
        }
    }
}
