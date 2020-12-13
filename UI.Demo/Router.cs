using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TheRuleOfSilvester.Core.SumTypes;
using TheRuleOfSilvester.UI.Drawing;
using TheRuleOfSilvester.UI.Inputs;
using UI.Demo.Views;

namespace UI.Demo
{
    public class Router : IDisposable
    {
        private readonly Graphic graphic;
        private readonly Input input;

        private readonly BehaviorSubject<View> focusable;

        public Router(Graphic graphic, Input input)
        {
            this.graphic = graphic;
            this.input = input;

            focusable = new BehaviorSubject<View>(default);
        }

        public IDisposable Show()
        {
            //var view = new View(input.ReadLine().Select(s => new ViewState(s)));
            //return view
            //    .Show()
            //    .SelectMany(s => s.Instructions)
            //    .MapMany(
            //        (IObservable<GraphicInstruction.WriteLine> writeLine) => graphic.WriteLine(writeLine),
            //        (IObservable<GraphicInstruction.Write> write) => graphic.Write(write),
            //        (IObservable<GraphicInstruction.SetPosition> setPosition) => graphic.CursorPosition(setPosition)
            //    ).Subscribe();
        }

        internal void SetFocus(View view) 
            => focusable.OnNext(view);

        internal IObservable<FocusState> ControlAsFocusable(View view) 
            => Observable.Merge(
                    input.ReadLine().Cast<FocusState>(),
                    input.ReadKey().Select(k => (FocusState)k),
                    focusable.Select(f => (FocusState)(f == view))
                    )
                .TakeWhile(str => view == focusable.Value)
                .RepeatWhen(objs => focusable.Where(v => v == view));

        public void Dispose()
        {
            focusable?.Dispose();
        }

        public class FocusState : Variant<FocusState.Focus, FocusState.PressedKey, FocusState.NewLine>
        {
            public FocusState(Focus value) : base(value)
            {
            }
            public FocusState(PressedKey value) : base(value)
            {
            }
            public FocusState(NewLine value) : base(value)
            {
            }

            public record Focus(bool HasFocus);
            public record PressedKey(ConsoleKeyInfo KeyInfo);
            public record NewLine(string Value);

            public static implicit operator FocusState(Focus value)
                => new(value);
            public static implicit operator FocusState(bool value)
                => new(new Focus(value));

            public static implicit operator FocusState(PressedKey value)
                => new(value);
            public static implicit operator FocusState(ConsoleKeyInfo value)
                => new(new PressedKey(value));

            public static implicit operator FocusState(NewLine value)
                => new(value);
            public static implicit operator FocusState(string value)
                => new(new NewLine(value));
        }


    }
}
