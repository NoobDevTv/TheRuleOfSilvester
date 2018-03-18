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
        public IMultiplayerComponent MultiplayerComponent { get; set; }

        public bool IsRunning { get; private set; }
        public bool IsMutliplayer { get; private set; }

        private int frame;
        private int ups;
        private Thread gameThread;
        private Map map;
        private List<Cell> cells;
        private Player player;
        //int c = 0;

        public void Run(int frame, int ups, bool multiplayer)
        {
            IsMutliplayer = multiplayer;
            this.frame = frame;
            this.ups = ups;
            //map = new Map();
            var generator = new MapGenerator();
            map = generator.Generate(20, 10);
            cells = new List<Cell> {
                new CornerLeftDown(map),
                new CornerLeftUp(map),
                new CornerRightDown(map),
                new CornerRightUp(map),
                new CrossLeftRightUpDown(map),
                new LeftDownRight(map),
                new LeftUpRight(map),
                new StraightLeftRight(map),
                new StraightUpDown(map),
                new UpDownLeft(map),
                new UpDownRight(map),
            };

            player = new Player(map) { Color = Color.Red, Name = "Me", Position = new Point(2, 1) };
            var character = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(character))
                character = "20050";
            player.SetAvatar(character[0]);
            map.Players.Add(player);

            IsRunning = true;

            if (multiplayer)
                MultiplayerComponent.Connect();

            gameThread = new Thread(Loop)
            {
                Name = "gameThread"
            };

            gameThread.Start();
        }
        public void Run(int frame, int ups) => Run(frame, ups, false);

        public void Stop()
        {
            if (IsMutliplayer)
                MultiplayerComponent.Disconnect();

            if (gameThread.IsAlive)
                IsRunning = false;
        }

        public void Update()
        {
            if (IsMutliplayer)
                MultiplayerComponent.Update(this);

            player.Update(this);

            DrawComponent.Draw(map);
            InputCompoment.LastKey = -1;
        }

        public void Dispose()
        {
            //Stop();

            gameThread = null;
        }

        private void Loop()
        {
            while (IsRunning)
            {
                Update();
                Thread.Sleep(1000 / frame);
            }
        }

    }
}
