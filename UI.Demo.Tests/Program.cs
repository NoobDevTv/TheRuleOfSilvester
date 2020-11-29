using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Threading;

using Subject<bool> focus = new();

var foc = false;

using var sub = focus.Subscribe(f => foc = f);

Observable.Range(0, 100)
.TakeWhile(CheckFocus)
.RepeatWhen(objs => objs.Select(o => foc).Where(f => f))
.Subscribe(i => Console.WriteLine(i), () => Console.WriteLine("is completed"));

do
{
    Console.ReadLine();
    focus.OnNext(true);

} while (true);

static bool CheckFocus(int i)
   => i < 50;
