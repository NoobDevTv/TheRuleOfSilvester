using System;
using System.Collections.Generic;
using System.Text;
using TheRuleOfSilvester.Runtime;

namespace TheRuleOfSilvester.MenuItems
{
    public abstract class MenuResult
    {
        public Type Type { get; }
        public object Content { get; }

        protected MenuResult(object content, Type type)
        {
            Type = type;
            Content = content;
        }
    }
    public sealed class MenuResult<T> : MenuResult
    {
        public new T Content => (T)base.Content;

        public MenuResult(T content) : base(content, typeof(T))
        {
        }
        
    }
}
