using System;
using System.Collections.Generic;
using System.Text;

namespace StreamSimulator.Cli.Flags
{
    public class PortFlag : CommandFlag
    {
        public PortFlag(string arg)
        {
            Type = FlagType.Integer;
        }
    }
}
