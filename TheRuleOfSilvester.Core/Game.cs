using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core.Cells;

namespace TheRuleOfSilvester.Core
{
    public class Game : IDisposable
    {
        public IDrawComponent DrawComponent { get; set; }
        public IInputCompoment InputCompoment { get; set; }

        private int frame;
        private int ups;
        private Thread gameThread;
        private Map map;
        private List<Cell> cells;
        private Player player;
        //int c = 0;

        public void Run(int frame, int ups)
        {
            this.frame = frame;
            this.ups = ups;
            //map = new Map();
            var generator = new MapGenerator();
            map = generator.Generate(10, 20);
            cells = new List<Cell> {
                new CornerLeftDown(map),
                new CornerLeftUp(map),
                new CornerRightDown(map),
                new CornerRightUp(map),
                new Cross(map),
                new LeftDownRight(map),
                new LeftUpRight(map),
                new StraightLeftRight(map),
                new StraightUpDown(map),
                new UpDownLeft(map),
                new UpDownRight(map),
            };

            player = new Player(map) { Color = Color.Green, Name = "Me", Position = new Point(1, 2) };
            var character = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(character))
                character = "20050";
            player.SetAvatar((char)ushort.Parse(character));
            map.Cells.Add(player);

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
            player.Update(this);



            DrawComponent.Draw(map);
            InputCompoment.LastKey = -1;
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
