using System;
using System.Collections.Generic;
using System.Text;

namespace StreamSimulator.Cli
{
    public interface ICliCommand
    {
        string Key { get; }
        IEnumerable<ICommandFlag> Flags { get; }
        CommandType Type { get; }
    }
}
