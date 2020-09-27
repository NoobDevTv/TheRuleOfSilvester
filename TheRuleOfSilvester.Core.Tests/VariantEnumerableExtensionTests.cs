using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TheRuleOfSilvester.Core.SumTypes;

namespace TheRuleOfSilvester.Core.Tests
{
    public class VariantEnumerableExtensionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleMapTest()
        {
            var list = new List<Variant<Demo1, Demo2>> { new Demo1(), new Demo2(), new Demo1() };
            list.Map((Demo2 i) => i.Call()).ToList();

            Assert.IsTrue(list.Where(v => v.Contains(typeof(Demo2))).Select(v => { v.Get(out Demo2 d2); return d2; }).All(x=>x.Called));
            Assert.IsFalse(list.Where(v => v.Contains(typeof(Demo1))).Select(v => { v.Get(out Demo1 d1); return d1; }).All(x=>x.Called));
        }


        private class Demo1 : Demo { }
        private class Demo2 : Demo { }

        private class Demo
        {
            public bool Called { get; private set; }

            public void Call()
            {
                Called = true;
            }
        }
    }
}