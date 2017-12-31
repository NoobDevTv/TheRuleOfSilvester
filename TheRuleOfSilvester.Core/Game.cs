using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    public class Game : IDisposable
    {
        public IDrawComponent DrawComponent { get; set; }

        private int frame;
        private int ups;
        private Thread gameThread;
        private Map map;
        private List<Cell> cells;
        int c = 0;

        public void Run(int frame, int ups)
        {
            this.frame = frame;
            this.ups = ups;
            map = new Map();
            cells = new List<Cell> {
                new CornerLeftDown(),
                new CornerLeftUp(),
                new CornerRightDown(),
                new CornerRightUp(),
                new Cross(),
                new LeftDownRight(),
                new LeftUpRight(),
                new StraightLeftRight(),
                new StraightUpDown(),
                new UpDownLeft(),
                new UpDownRight(),
            };

            gameThread = new Thread(Loop)
            {
                Name = "gameThread"
            };

            gameThread.Start();
        }

        public void Stop()
        {
            if (gameThread.IsAlive)
                gameThread.Abort();
        }

        public void Update()
        {
            DrawComponent.Draw(map);
            cells[c].Invalid = true;
            map.Cells[3, 3] = cells[c++];
            c %= cells.Count;
        }

        public void Dispose()
        {
            //Stop();

            //gameThread = null;
        }

        private void Loop()
        {
            while (true)
            {
                Update();
                Thread.Sleep(1000 / frame);
            }
        }
    }
}
