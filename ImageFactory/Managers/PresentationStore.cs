using ImageFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageFactory.Managers
{
    // This is probably some of the shadiest code I've written in a while ngl (PART 1)
    internal class PresentationStore
    {
        private readonly IEnumerable<float> _oneToOneHundred;
        private readonly IEnumerable<Value> _presentationValues;

        public PresentationStore()
        {
            _oneToOneHundred = Enumerable.Range(5, 95).Select(f => decimal.ToSingle((decimal)Math.Round(f * 0.01d, 1)));
            var xcast = Enumerable.Range(1, 95).Select(i => (object)(i * 10)).ToList();
            var casted = _oneToOneHundred.Cast<object>().ToList();

            _presentationValues = new List<Value>
            {
                new Value("Everywhere"),
                new Value("In Menu"),
                new Value("Results Screen", false, new ValueConstructor("When", "Finished", new List<object> { "Finished", "Passed", "Failed" })),
                new Value("In Song"),
                new Value("%", false, new ValueConstructor("When", "Below", new List<object> { "Below", "Above" }), new ValueConstructor("%", 0.8f, casted)),
                new Value("% Range", false, new ValueConstructor("When Above (%)", 0.8f, casted), new ValueConstructor("and Below (%)", 0.9f, casted)),
                new Value("Combo", true, new ValueConstructor("On Combo", 100, xcast)),
                new Value("Combo Increment", true, new ValueConstructor("On Every X Combo", 100, xcast)),
                new Value("Combo Drop", true),
                new Value("On Last Note", true)
            };
        }

        public IEnumerable<Value> Values()
        {
            return _presentationValues;
        }

        public class Value
        {
            public string ID { get; }
            public bool HasDuration { get; }
            public IEnumerable<ValueConstructor> Constructors { get; }

            public Value(string id, bool hasDuration = false)
            {
                ID = id;
                HasDuration = hasDuration;
                Constructors = Array.Empty<ValueConstructor>();
            }

            public Value(string id, bool hasDuration, ValueConstructor constructor)
            {
                ID = id;
                HasDuration = hasDuration;
                Constructors = new ValueConstructor[] { constructor };
            }

            public Value(string id, bool hasDuration, params ValueConstructor[] constructors)
            {
                ID = id;
                HasDuration = hasDuration;
                Constructors = constructors;
            }
        }
    }
}