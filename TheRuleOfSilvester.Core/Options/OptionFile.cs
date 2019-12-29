using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Options
{
    public sealed class OptionFile
    {
        public Dictionary<string, Option> Options { get; set; }

        public OptionFile()
        {
            Options = new Dictionary<string, Option>();
        }
    }
}
