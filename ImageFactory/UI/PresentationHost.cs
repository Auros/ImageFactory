using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImageFactory.UI
{
    // This is probably some of the shadiest code I've written in a while ngl (PART 2)
    internal class PresentationHost
    {
        [UIValue("presentation-options")]
        protected readonly List<object> _presentationOptions;

        [UIComponent("presentation-list")]
        protected readonly CustomCellListTableData _presentationList = null!;

        private string _lastID = "";

        public PresentationHost(PresentationStore store)
        {
            _presentationOptions = store.Values().Cast<object>().ToList();
        }

        // LITERALLY PRETEND THIS DOES NOT EXIST
        public Tuple<string, string, float?> Export()
        {
            var hosts = _presentationList.data.Cast<InternalHost>();
            var duration = hosts.FirstOrDefault(h => h.isDuration);
            var agg = (hosts.Count() == 0 || (hosts.Count() == 1 && duration != null)) ? ""
                : hosts.Where(h => !h.isDuration).Select(f => f.Value.ToString()).Aggregate((f, n) => f + "|" + n);
            
            return new Tuple<string, string, float?>(_lastID, agg, duration?.Duration);
        }

        [UIAction("format")]
        protected string FormatOptions(PresentationStore.Value value)
        {
            return value.ID;
        }

        [UIAction("changed")]
        protected void Changed(PresentationStore.Value value)
        {
            _presentationList.data.Clear();
            _presentationList.tableView.ReloadData();

            foreach (Transform pres in _presentationList.tableView.contentTransform)
                UnityEngine.Object.Destroy(pres.gameObject);

            _presentationList.data.AddRange(value.Constructors.Select(c => new InternalHost(c)));
            if (value.HasDuration) _presentationList.data.Add(new InternalHost());
            _presentationList.tableView.ReloadData();
            _lastID = value.ID;
        }

        private class InternalHost
        {
            [UIValue("is-list")]
            public readonly bool isList;

            [UIValue("is-duration")]
            public readonly bool isDuration;

            [UIValue("name")]
            public readonly string name;

            [UIValue("selected-value")]
            public object Value { get; set; }

            [UIValue("options")]
            protected readonly List<object> options;

            [UIValue("duration")]
            public float Duration { get; set; } = 1f;

            public InternalHost(ValueConstructor constructor)
            {
                isList = true;
                isDuration = false;
                name = constructor.Name;
                options = constructor.Values;
                Value = constructor.DefaultValue;
            }

            public InternalHost()
            {
                Value = 1f;
                isList = false;
                name = "Duration";
                isDuration = true;
                options = new List<object>();
            }

            [UIAction("formatter")]
            protected string Formatter(object value)
            {
                if (name.Contains("%"))
                {
                    return string.Format("{0:0%}", value);
                }
                return value.ToString();
            }

            [UIAction("dur-format")]
            protected string DurationFormatter(float value)
            {
                if (value == 1f)
                    return "1 second";
                return value.ToString() + " seconds";
            }
        }
    }
}