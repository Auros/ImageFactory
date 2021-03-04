using ImageFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageFactory.Managers
{
    internal class PresentationStore
    {
        private readonly IEnumerable<float> _oneToOneHundred;
        private readonly IEnumerable<Value> _presentationValues;

        public PresentationStore()
        {
            _oneToOneHundred = Enumerable.Range(5, 100).Cast<float>().Select(f => f * 0.01f);
            _presentationValues = new List<Value>
            {
                new Value("Everywhere"),
                new Value("In Menu"),
                new Value("Results", new ValueConstructor<object>("Finished", "Finished", "Passed", "Failed")),
                new Value("In Song"),
                new Value("%", new ValueConstructor<object>("Before", "Before", "After"), new ValueConstructor<object>("", 0.8f, _oneToOneHundred)),
                new Value("% Range", new ValueConstructor<object>("When Above", 0.8f, _oneToOneHundred), new ValueConstructor<object>("and Below", 0.9f, _oneToOneHundred)),
                new Value("Combo", new ValueConstructor<object>("", 100, Enumerable.Range(1, 100).Select(i => i * 10))),
                new Value("Combo Increment", new ValueConstructor<object>("On Every X Combo", 100, Enumerable.Range(1, 100).Select(i => i * 10))),
                new Value("Combo Drop"),
                new Value("On Last Note")
            };
        }

        public IEnumerable<Value> Values()
        {
            return _presentationValues;
        }

        public class Value
        {
            public string ID { get; }
            public IEnumerable<ValueConstructor<object>> Constructors { get; }

            public Value(string id)
            {
                ID = id;
                Constructors = Array.Empty<ValueConstructor<object>>();
            }

            public Value(string id, ValueConstructor<object> constructor)
            {
                ID = id;
                Constructors = new ValueConstructor<object>[] { constructor };
            }

            public Value(string id, params ValueConstructor<object>[] constructors)
            {
                ID = id;
                Constructors = constructors;
            }
        }
    }
}