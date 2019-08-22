using System;
using System.Collections.Generic;
using System.Text;

namespace StreamSimulator.Cli
{
    public class CliCommand : ICliCommand
    {
        public string Key { get; set; }
        public CommandType Type { get; protected set; }
        public IEnumerable<ICommandFlag> Flags { get; internal set; }
    }
}
