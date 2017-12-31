using System;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester
{
    class Program
    {
        static Game game;
        static AutoResetEvent are;
        //┌┬┐└┴┘│├┼┤
        static void Main(string[] args)
        {
            are = new AutoResetEvent(false);
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;
            using (game = new Game())
            {
                game.DrawComponent = new DrawComponent();
                game.Run(60, 60);
                are.WaitOne();
            }
        }
    }
}
