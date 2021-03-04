using System;

namespace ImageFactory.Models
{
    internal class ValueConstructor<T>
    {
        public T[] Values { get; }
        public string Name { get; }
        public T DefaultValue { get; }

        public ValueConstructor(string name, T defaultValue, params T[] values)
        {
            Name = name;
            Values = values;
            DefaultValue = defaultValue;
        }

        public new Type GetType() => DefaultValue!.GetType();
    }
}