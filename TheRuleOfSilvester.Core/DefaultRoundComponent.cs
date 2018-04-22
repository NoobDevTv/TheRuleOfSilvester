using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;

namespace TheRuleOfSilvester.Core
{
    public class DefaultRoundComponent : IRoundComponent
    {
        public uint Round => throw new NotImplementedException();
        public RoundMode RoundMode { get; set; }

        private int maxRoundMode;
        private TextCell roundModeCell;

        public DefaultRoundComponent(Map map)
        {
            maxRoundMode = Enum.GetValues(typeof(RoundMode)).Cast<int>().Max()+1;
            roundModeCell = new TextCell(("RoundMode: " + RoundMode).PadRight(20, ' '),20, map) { Position = new Point(4, (map.Height*3)+3) };
            map.TextCells.Add(roundModeCell);
        }

        public void Update(Game game)
        {
            if (!game.InputCompoment.RoundButton)
                return;

            roundModeCell.Text = ("RoundMode: " + RoundMode).PadRight(20,' ');

            RoundMode += 1;
            RoundMode = (RoundMode)((int)RoundMode % maxRoundMode);
        }
    }
}
