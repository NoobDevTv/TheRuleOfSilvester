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
        private Player player;

        public void Run(int frame, int ups, bool multiplayer)
        {
            IsMutliplayer = multiplayer;

            if (multiplayer)
                MultiplayerComponent.Connect();

            this.frame = frame;
            this.ups = ups;

            if (multiplayer)
            {
                map = MultiplayerComponent.GetMap();
            }
            else
            {
                var generator = new MapGenerator();
                map = generator.Generate(20, 10);
            }

            player = new Player(map) { Color = Color.Red, Name = "Me", Position = new Point(2, 1) };
            var character = Console.ReadLine(); //TODO move char to UI
            if (string.IsNullOrWhiteSpace(character))
                character = "20050";
            player.SetAvatar(character[0]);
            map.Players.Add(player);

            IsRunning = true;

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
