using System;
using System.Text;
using System.Threading;
using TheRuleOfSilvester.Core;

namespace TheRuleOfSilvester
{
    class Program
    {
        
        static Game game;
        //static AutoResetEvent are;
        static InputComponent inputComponent;
        //┌┬┐└┴┘│├┼┤
        static void Main(string[] args)
        {
            //are = new AutoResetEvent(false);
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;
            
            inputComponent = new InputComponent();
            
            using (game = new Game())
            {
                game.DrawComponent = new DrawComponent();
                game.InputCompoment = inputComponent;
                game.Run(60, 60);
                inputComponent.Listen();
                //are.WaitOne();
            }
        }
    }
}
