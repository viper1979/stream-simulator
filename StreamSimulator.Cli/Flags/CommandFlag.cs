using System;
using System.Collections.Generic;
using System.Text;

namespace StreamSimulator.Cli
{
    public enum FlagType
    {
        Integer,
        String
    }

    public class CommandFlag : ICommandFlag
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public FlagType Type { get; internal set; }
    }
}
