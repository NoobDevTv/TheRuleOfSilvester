using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Core.Options
{
    [JsonConverter(typeof(OptionConverter))]
    public readonly struct Option
    {
        [JsonConverter(typeof(OptionValueConverter))]
        public readonly object Value { get;  }

        [JsonConstructor]
        public Option(object value)
        {
            Value = value;
        }
    }
}
