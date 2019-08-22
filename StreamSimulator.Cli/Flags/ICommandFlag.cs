using System;
using System.Collections.Generic;
using System.Text;

namespace StreamSimulator.Cli
{
    public interface ICommandFlag
    {
        string Key { get; }
        object Value { get; }
    }
}
