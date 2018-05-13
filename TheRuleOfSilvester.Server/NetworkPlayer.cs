using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Server
{
    public class NetworkPlayer
    {
        public Player Player { get; set; }

        public NetworkPlayer(Player player)
        {
            Player = player;
        }

    }
}
