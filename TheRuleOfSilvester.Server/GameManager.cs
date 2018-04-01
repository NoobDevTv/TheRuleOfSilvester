using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester.Server
{
    static class GameManager
    {
        public static Map Map { get; private set; }

        static GameManager()
        {
           Map = GenerateMap();
        }

        private static Map GenerateMap() => new MapGenerator().Generate(20, 20);
    }
}
