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
            using var reset = new ManualResetEvent(false);
            var input = new Input(ConsoleDriver.ReadKey, ConsoleDriver.ReadLine);
            var graphic = new Graphic(
                ConsoleDriver.Write,
                ConsoleDriver.WriteLine,
                ConsoleDriver.ForegroundColor,
                ConsoleDriver.BackgroundColor,
                ConsoleDriver.SetCursorPosition);
            using var router = new Router(graphic, input);
            using var disp = router.Show();

            reset.WaitOne();
        }
    }
}
