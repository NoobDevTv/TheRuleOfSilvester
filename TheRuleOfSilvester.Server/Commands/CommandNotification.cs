using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core.Observation;
using TheRuleOfSilvester.Network;

namespace TheRuleOfSilvester.Server
{
    public class CommandNotification
    {
        public CommandName CommandName { get; set; }
        public Notification Notification { get; set; }
        public BaseClient Client { get; set; }     
    }
}
