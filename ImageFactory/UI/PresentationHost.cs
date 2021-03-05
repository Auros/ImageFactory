using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using ImageFactory.Managers;
using ImageFactory.Models;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImageFactory.UI
{
    // This is probably some of the shadiest code I've written in a while ngl (PART 2)
    // Use any other part of this mod as an example but this
    internal class PresentationHost
    {
        private PresentationStore.Value _activeValue;

        [UIValue("active-presentation")]
        protected internal PresentationStore.Value Value
        {
            get => _activeValue;
            set
            {
                _activeValue = value;
                Changed(_activeValue);
            }
        }
        
        [UIValue("presentation-options")]
        protected List<object> _presentationOptions;

        [UIComponent("dropdown")]
        protected readonly DropDownListSetting _dropdown = null!;

        [UIParams]
        protected readonly BSMLParserParams _parserParams = null!;

        [UIComponent("presentation-list")]
        protected readonly CustomCellListTableData _presentationList = null!;

        private string _lastID = "";
        private readonly PresentationStore _store;

        private bool _justSet = false;
        private ImagePresentationData? _presentation;
        public ImagePresentationData? LastData { get => _presentation; set { _justSet = true; _presentation = value; } }

        public PresentationHost(PresentationStore store)
        {
            _store = store;
            var values = store.Values();
            _activeValue = values.First();
            _presentationOptions = values.Cast<object>().ToList();
        }

        [UIAction("format")]
        protected string FormatOptions(PresentationStore.Value value)
        {
            return value.ID;
        }

        public void Update()
        {
            if (LastData != null)
            {
                var val = _presentationOptions.Cast<PresentationStore.Value>().FirstOrDefault(p => p.ID == LastData.PresentationID);
                if (val != null) Value = val;
            }
        }

        public void Reset()
        {
            LastData = null;
            _lastID = "";
        }

        // WHAT IS WRONG WITH ME???
        protected void Changed(PresentationStore.Value value)
        {
            if (_lastID == value.ID)
                return;

            _activeValue = value;
            _presentationList.data.Clear();
            _presentationList.tableView.ReloadData();

            foreach (Transform pres in _presentationList.tableView.contentTransform)
                UnityEngine.Object.Destroy(pres.gameObject);

            _presentationOptions = _store.Values().Cast<object>().ToList();
            if (LastData != null && (_justSet || (!_justSet && LastData.PresentationID == _activeValue.ID)) )
            {
                var id = LastData.PresentationID;
                var presVal = _presentationOptions.Cast<PresentationStore.Value>().FirstOrDefault(p => p.ID == id);
                if (presVal != null)
                {
                    _activeValue = presVal;
                    var presList = _store.Values().ToArray();
                    var index = presList.IndexOf(presVal);

                    if (index >= 0)
                    {
                        var reconstructors = new List<ValueConstructor>();
                        var valCount = presVal.Constructors.Count();
                        var props = LastData.Value.Split('|');
                        var propCount = props.Length;

                        if (valCount > 0 && valCount == propCount)
                        {
                            for (int i = 0; i < valCount; i++)
                            {
                                var saveVal = props[i];
                                object reconstructedValue = saveVal;
                                var con = presVal.Constructors.ElementAt(i);

                                if (float.TryParse(saveVal, out float reFloat))
                                    reconstructedValue = reFloat;
                                if (int.TryParse(saveVal, out int reInt))
                                    reconstructedValue = reInt;
                                reconstructors.Add(new ValueConstructor(con.Name, reconstructedValue, con.Values));
                            }
                        }
                        var clone = new PresentationStore.Value(id, presVal.HasDuration, reconstructors.ToArray());
                        presList[index] = clone;
                        _dropdown.Value = clone;
                        _activeValue = clone;
                    }
                    _presentationOptions = presList.Cast<object>().ToList();
                }
            }
            else
            {
                _presentationOptions = _store.Values().Cast<object>().ToList();
            }

            foreach (Transform pres in _presentationList.tableView.contentTransform)
                UnityEngine.Object.Destroy(pres.gameObject);

            _presentationList.data.AddRange(_activeValue.Constructors.Select(c => new InternalHost(c)));
            if (_activeValue.HasDuration)
            {
                var durHost = new InternalHost();
                _presentationList.data.Add(durHost);
                if (_activeValue != null && _activeValue.HasDuration && LastData != null && LastData.Duration != 0)
                    durHost.Duration = LastData.Duration;
            }
            _presentationList.tableView.ReloadData();
            _lastID = _activeValue!.ID;

            _dropdown.values = _presentationOptions;
            _parserParams.EmitEvent("get");
            _dropdown.UpdateChoices();
            _justSet = false;
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