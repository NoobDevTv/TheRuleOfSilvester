using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Network.Observation
{
    public class Subscription : IDisposable
    {
        public IObserver<Package> Observer { get; private set; }
        public IObservable<Package> Observable { get; private set; }

        public Subscription(IObserver<Package> observer, IObservable<Package> observable)
        {
            Observer = observer;
            Observable = observable;
        }        

        public void Dispose()
        {
            Observer.OnCompleted();
            Observer = null;
            Observable = null;
        }
    }
}
