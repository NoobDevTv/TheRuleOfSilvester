using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;
using TheRuleOfSilvester.Core.RoundComponents;
using System.Reflection;

namespace TheRuleOfSilvester.Core
{
    public class DefaultRoundManagerComponent : IRoundManagerComponent
    {
        public IRoundComponent Round { get; private set; }
        public RoundMode RoundMode { get; set; }

        private int maxRoundMode;
        private TextCell roundModeCell;
        private List<IRoundComponent> rounds;

        public DefaultRoundManagerComponent(Map map)
        {
            rounds = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IRoundComponent))).Select(x => (IRoundComponent)Activator.CreateInstance(x)).ToList();
            maxRoundMode = Enum.GetValues(typeof(RoundMode)).Cast<int>().Max() + 1;
            roundModeCell = new TextCell(("RoundMode: " + RoundMode).PadRight(20, ' '), 20, map) { Position = new Point(4, (map.Height * 3) + 3) };
            map.TextCells.Add(roundModeCell);
        }

        public void Update(Game game)
        {
            if (Round != null)
                Round.Update(game);

            if (!game.InputCompoment.RoundButton)
                return;

            if (Round != null)
                Round.Stop(game);

            RoundMode += 1;
            RoundMode = (RoundMode)((int)RoundMode % maxRoundMode);

            Round = rounds.FirstOrDefault(x => x.Round == RoundMode);

            if (Round != null)
                Round.Start(game);

            roundModeCell.Text = ("RoundMode: " + RoundMode).PadRight(20, ' ');
        }
    }
}
