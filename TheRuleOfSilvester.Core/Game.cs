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
        public IRoundComponent RoundComponent { get; set; }

        public Map Map { get; private set; }

        public bool IsRunning { get; private set; }
        public bool IsMutliplayer { get; private set; }

        private int frame;
        private int ups;
        private Thread gameThread;
        private Player player;

        public Game()
        {
        }

        public void Run(int frame, int ups, bool multiplayer)
        {
            IsMutliplayer = multiplayer;

            if (multiplayer)
                MultiplayerComponent.Connect();

            this.frame = frame;
            this.ups = ups;

            if (multiplayer)
            {
                Map = MultiplayerComponent.GetMap();
            }
            else
            {
                var generator = new MapGenerator();
                Map = generator.Generate(20, 10);
            }

            player = new Player(Map) { Color = Color.Red, Name = Guid.NewGuid().ToString(), Position = new Point(2, 1) };
            var character = Console.ReadLine(); //TODO move char to UI
            if (string.IsNullOrWhiteSpace(character))
                character = "20050";
            player.SetAvatar(character[0]);
            Map.Players.Add(player);

            if (RoundComponent == null)
                RoundComponent = new DefaultRoundComponent(Map);

            IsRunning = true;

            if (multiplayer)
            {
                MultiplayerComponent.RegisterNewPlayer(player);
            }

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

            RoundComponent.Update(this);

            DrawComponent.Draw(Map);
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
