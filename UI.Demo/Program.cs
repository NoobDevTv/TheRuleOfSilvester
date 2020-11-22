using System;
using System.Threading;
using TheRuleOfSilvester.UI.Drawing;
using TheRuleOfSilvester.UI.Inputs;

namespace UI.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var reset = new ManualResetEvent(false);
            var input = new Input(ConsoleDriver.ReadKey, ConsoleDriver.ReadLine);
            var graphic = new Graphic(
                ConsoleDriver.Write,
                ConsoleDriver.WriteLine,
                ConsoleDriver.ForegroundColor,
                ConsoleDriver.BackgroundColor,
                ConsoleDriver.SetCursorPosition);
            var router = new Router(graphic, input);
            router.Show();

            reset.WaitOne();
        }
    }
}
