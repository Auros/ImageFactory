using ImageFactory.Models;
using ImageFactory.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageFactory.Managers
{
    // This is probably some of the shadiest code I've written in a while ngl (PART 1)
    // Use any other part of this mod as an example but this
    internal class PresentationStore
    {
        private readonly IEnumerable<float> _oneToOneHundred;
        private readonly IEnumerable<Value> _presentationValues;

        public PresentationStore()
        {
            _oneToOneHundred = Enumerable.Range(5, 95).Select(f => (float)decimal.ToDouble((decimal)Math.Round(f * 0.01d, 2)));
            var xcast = Enumerable.Range(1, 95).Select(i => (object)(i * 10)).ToList();
            var casted = _oneToOneHundred.Cast<object>().ToList();

            _presentationValues = new List<Value>
            {
                new Value(ScenePresenter.EVERYWHERE_ID),
                new Value(ScenePresenter.MENU_ID),
                new Value(ResultsPresenter.RESULTS_ID, false, new ValueConstructor("When", "Finished", new List<object> { "Finished", "Passed", "Failed" })),
                new Value(ScenePresenter.GAME_ID),
                new Value(PercentPresenter.PERCENT_ID, false, new ValueConstructor("When", "Below", new List<object> { "Below", "Above" }), new ValueConstructor("%", 0.8f, casted)),
                new Value(PercentPresenter.PERCENT_RANGE_ID, false, new ValueConstructor("When Above (%)", 0.8f, casted), new ValueConstructor("and Below (%)", 0.9f, casted)),
                new Value(ComboPresenter.COMBO_ID, true, new ValueConstructor("On Combo", 100, xcast)),
                new Value(ComboPresenter.COMBO_INCREMENT, true, new ValueConstructor("On Every X Combo", 100, xcast)),
                new Value(ComboPresenter.COMBO_HOLD, false, new ValueConstructor("When", "Above", new List<object> { "Below", "Above" }), new ValueConstructor("Combo", 100, xcast)),
                new Value(ComboPresenter.COMBO_DROP, true),
                new Value(FullComboPresenter.FULLCOMBO_ID, false),
                new Value(PausePresenter.PAUSE_ID, false),
                new Value(LastNotePresenter.LASTNOTE_ID, true)
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