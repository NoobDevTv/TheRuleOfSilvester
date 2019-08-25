using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public class CommandNotification
    {
        public CommandName CommandName { get; set; }
        public CommandArgs Arguments { get; set; }
    }
}
