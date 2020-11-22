using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core.SumTypes;
using TheRuleOfSilvester.UI.Drawing;
using TheRuleOfSilvester.UI.Inputs;

namespace UI.Demo
{
    public class Router
    {
        private readonly Graphic graphic;
        private readonly Input input;

        public Router(Graphic graphic, Input input)
        {
            this.graphic = graphic;
            this.input = input;
        }


        public IDisposable Show()
        {
            var view = new View(input.ReadLine().Select(s => new ViewState(s)));
            return view
                .Show()
                .SelectMany(s => s.Instructions)
                .MapMany(
                    (IObservable<GraphicInstruction.WriteLine> writeLine) => graphic.WriteLine(writeLine.Select(w => w.Value)).Select(v => new ConsoleState(v)),
                    (IObservable<GraphicInstruction.Write> write) => graphic.Write(write.Select(w => w.Value)).Select(v => new ConsoleState(v)),
                    (IObservable<GraphicInstruction.SetPosition> setPosition) => graphic.CursorPosition(setPosition.Select(p => p.Value)).Select(v => new ConsoleState(v))
                ).Subscribe();
        }
    }
}
