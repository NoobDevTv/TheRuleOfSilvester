using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRuleOfSilvester.UI.Inputs
{
    public class Input
    {
        private readonly Func<IObservable<string>, IObservable<string>> writeLineFunc;
        private readonly Func<IObservable<string>, IObservable<string>> writeFunc;
        private readonly Func<IObservable<ConsoleKeyInfo>> readKeyFunc;
        private readonly Func<IObservable<string>> readLineFunc;

        public Input(Func<IObservable<string>, IObservable<string>> writeLine,
            Func<IObservable<string>, IObservable<string>> write, 
            Func<IObservable<ConsoleKeyInfo>> readKey, 
            Func<IObservable<string>> readLine)
        {
            writeLineFunc = writeLine;
            writeFunc = write;
            readKeyFunc = readKey;
            readLineFunc = readLine;
        }

        public IObservable<string> WriteLine(IObservable<string> strings)
            => writeLineFunc(strings);
        public IObservable<string> Write(IObservable<string> strings)
            => writeFunc(strings);
        public IObservable<ConsoleKeyInfo> ReadKey()
            => readKeyFunc();
        public IObservable<string> ReadLine()
            => readLineFunc();

    }
}
