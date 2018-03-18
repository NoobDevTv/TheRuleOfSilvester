using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester
{
    class Program
    {
        private static Game game;
        private static InputComponent inputComponent;
        private static GameMenu menu;

        public static bool IsRunning { get; private set; }

        //┌┬┐└┴┘│├┼┤
        static void Main(string[] args)
        {
            //are = new AutoResetEvent(false);
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;
            inputComponent = new InputComponent();
            IsRunning = true;

            menu = new GameMenu(new List<MenuItem>()
            {
               new MenuItem(true, "New Game", SinglePlayer),
               new MenuItem(false, "Multiplayer", MultiPlayer),
               new MenuItem(false, "Exit", () => false)
            });

            while (IsRunning)
            {
                var menuItem = menu.Run();
                IsRunning = menuItem.Action();
            }
        }

        private static bool MultiPlayer()
        {
            throw new NotImplementedException();
        }

        private static bool SinglePlayer()
        {
            Console.Clear();

            using (game = new Game())
            {
                game.DrawComponent = new DrawComponent();
                game.InputCompoment = inputComponent;
                game.MultiplayerComponent = new MultiplayerComponent();
                game.Run(60, 60);
                inputComponent.Listen();
                game.Stop();
            }

            return true;
        }
    }
}
