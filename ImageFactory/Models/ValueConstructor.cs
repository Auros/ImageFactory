using System;
using System.Collections.Generic;

namespace ImageFactory.Models
{
    internal class ValueConstructor
    {
        public string Name { get; }
        public object DefaultValue { get; }
        public List<object> Values { get; }

        public ValueConstructor(string name, object defaultValue, List<object> values)
        {
            Name = name;
            Values = values;
            DefaultValue = defaultValue;
        }

        public new Type GetType() => DefaultValue!.GetType();
    }
}