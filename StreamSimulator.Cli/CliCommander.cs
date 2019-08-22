using Microsoft.Extensions.Configuration;
using StreamSimulator.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace StreamSimulator.Cli
{
    public class CliCommander
    {
        static void Main(string[] args)
        {
            new CliCommander(args);
        }

        /***/

        private StreamManager _streamManager;

        public CliCommander(string[] args)
        {
            LoadConfiguration();

            var command = GetCommand(args);

            _streamManager = new StreamManager();

            switch(command.Type)
            {
                case CommandType.Start:
                    {
                        var startCommand = (StartCommand)command;
                        var server = _streamManager.Add(startCommand.Settings);
                        server.Start();
                        break;
                    }
                case CommandType.Stop:
                    {
                        _streamManager.Stop(int.Parse(args[1]));
                        break;
                    }
            }
        }

        private ICliCommand GetCommand(string[] args)
        {
            switch(args[0].ToLower())
            {
                case "start":
                    {
                        return new StartCommand();
                    }
                case "stop":
                    {
                        return new StopCommand();
                    }
                default:
                    break;
            }

            return null;
        }

        private IEnumerable<ICommandFlag> GetFlags(string[] args)
        {
            return new[] { new CommandFlag() { Key = args[1] } };
        }

        private IConfigurationRoot LoadConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.*.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            return configuration;    
        }
    }
}
