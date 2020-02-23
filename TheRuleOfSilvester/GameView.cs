using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester
{
    public sealed class GameView
    {
        public Game Game { get; set; }

        public void Run()
            => Game.Wait();
    }
}
