using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.UI.Inputs
{
    public class Input
    {
        private readonly Func<IObservable<ConsoleKeyInfo>> readKeyFunc;
        private readonly Func<IObservable<string>> readLineFunc;

        public Input(
            Func<IObservable<ConsoleKeyInfo>> readKey, 
            Func<IObservable<string>> readLine)
        {
            readKeyFunc = readKey;
            readLineFunc = readLine;
        }

        public IObservable<ConsoleKeyInfo> ReadKey()
            => readKeyFunc();
        public IObservable<string> ReadLine()
            => readLineFunc();
    }
}
