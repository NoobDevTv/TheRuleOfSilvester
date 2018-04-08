using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Server.Commands
{
    public partial class MapCommands
    {
        [Command((short)1)]
        public static byte[] GetMap(byte[] data) => SerializeHelper.ToByteArray(GameManager.Map);
    }
}
