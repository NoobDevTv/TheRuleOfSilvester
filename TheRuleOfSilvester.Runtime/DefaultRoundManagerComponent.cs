using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using TheRuleOfSilvester.Core;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Runtime.RoundComponents;

namespace TheRuleOfSilvester.Runtime
{
    public class DefaultRoundManagerComponent : IRoundManagerComponent
    {
        public IRoundComponent Round { get; private set; }
        public RoundMode RoundMode { get; set; }

        private readonly int maxRoundMode;
        private TextCell roundModeCell;
        private List<IRoundComponent> rounds;

        private bool firstRun;

        public DefaultRoundManagerComponent(Map map)
        {
            rounds = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterfaces().Contains(typeof(IRoundComponent)))
                .Select(x => (IRoundComponent)Activator.CreateInstance(x))
                .ToList();

            maxRoundMode = Enum.GetValues(typeof(RoundMode)).Cast<int>().Max() + 1;
            Round = rounds.FirstOrDefault(x => x.Round == RoundMode);
            firstRun = true;

            roundModeCell = new TextCell(("RoundMode: " + RoundMode).PadRight(20, ' '), 20, map)
            { Position = new Position(4, (map.Height * 3) + 3) };

            map.TextCells.Add(roundModeCell);
        }

        public void Update(Game game)
        {
            if (firstRun)
            {
                Round.Start(game);
                firstRun = false;
            }

            Round?.Update(game);

            if (game.InputAction?.Type != InputActionType.RoundButton &&
                Round != null &&
                !Round.RoundEnd)
                return;
            
            Round?.Stop(game);
            Round.RoundEnd = false;

            RoundMode += 1;
            RoundMode = (RoundMode)((int)RoundMode % maxRoundMode);

            Round = rounds.FirstOrDefault(x => x.Round == RoundMode);

            roundModeCell.Text = ("RoundMode: " + RoundMode).PadRight(20, ' ');

            Round?.Start(game);
        }
    }
}
