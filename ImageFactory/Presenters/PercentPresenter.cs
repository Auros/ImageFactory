using ImageFactory.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace ImageFactory.Presenters
{
    internal class PercentPresenter : IInitializable, IDisposable
    {
        private readonly Config _config;
        private readonly ImageManager _imageManager;
        private readonly List<SaveImage> _savedImages = new List<SaveImage>();
        private readonly RelativeScoreAndImmediateRankCounter _relativeScoreAndImmediateRankCounter;
        private readonly Dictionary<SaveImage, IFSprite> _activeSprites = new Dictionary<SaveImage, IFSprite>();

        public const string PERCENT_ID = "%";
        public const string PERCENT_RANGE_ID = "% Range";
        private readonly float _bufferTime = 0.2f;
        private DateTime _lastTime;

        public PercentPresenter(Config config, ImageManager imageManager, RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRankCounter)
        {
            _config = config;
            _imageManager = imageManager;
            _relativeScoreAndImmediateRankCounter = relativeScoreAndImmediateRankCounter;
        }

        public async void Initialize()
        {
            // Parse % Presentation Data
            var saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == PERCENT_ID);
            foreach (var save in saves)
            {
                IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
                if (metadata.HasValue)
                {
                    _imageManager.Spawn(save);
                    var image = await _imageManager.LoadImage(metadata.Value);
                    if (image != null)
                    {
                        string[] split = save.Presentation.Value.Split('|');
                        if (split.Length != 2)
                            continue;
                        if (Enum.TryParse(split[0], out Mode mode) && float.TryParse(split[1], out float val))
                        {
                            float? minVal = null;
                            if (mode == Mode.Above)
                                minVal = val;
                            float? maxVal = null;
                            if (mode == Mode.Below)
                                maxVal = val;

                            // If for some reason we dont have a min or a max, move onto the next save (manual config editing gone wrong, most likely)
                            if (!minVal.HasValue && !maxVal.HasValue)
                                continue;

                            var saveImage = new SaveImage(image, save, mode, minVal, maxVal);
                            _savedImages.Add(saveImage);
                        }
                    }
                }
            }

            // Parse % Range Presentation Data
            saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == PERCENT_RANGE_ID);
            foreach (var save in saves)
            {
                IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
                if (metadata.HasValue)
                {
                    _imageManager.Spawn(save);
                    var image = await _imageManager.LoadImage(metadata.Value);
                    if (image != null)
                    {
                        string[] split = save.Presentation.Value.Split('|');
                        if (split.Length != 2)
                            continue;
                        if (float.TryParse(split[0], out float minVal) && float.TryParse(split[1], out float maxVal))
                        {
                            var saveImage = new SaveImage(image, save, Mode.Between, minVal, maxVal);
                            _savedImages.Add(saveImage);
                        }
                    }
                }
            }
            _relativeScoreAndImmediateRankCounter.relativeScoreOrImmediateRankDidChangeEvent += RelativeScoreAndImmediateRankCounter_relativeScoreOrImmediateRankDidChangeEvent;
        }

        private void RelativeScoreAndImmediateRankCounter_relativeScoreOrImmediateRankDidChangeEvent()
        {
            if (_lastTime.AddSeconds(_bufferTime) > DateTime.Now)
                return;

            _lastTime = DateTime.Now;
            foreach (var image in _savedImages)
            {
                bool isValid = false;
                if (image.Mode == Mode.Between)
                {
                    // If for some reason we dont have a min or a max, just don't do anything.
                    if (!image.Min.HasValue || !image.Max.HasValue)
                        continue;
                    isValid = _relativeScoreAndImmediateRankCounter.relativeScore >= image.Min && image.Max >= _relativeScoreAndImmediateRankCounter.relativeScore;
                }
                else
                {
                    if (image.Mode == Mode.Above && image.Min.HasValue)
                        isValid = _relativeScoreAndImmediateRankCounter.relativeScore >= image.Min;
                    else if (image.Mode == Mode.Below && image.Max.HasValue)
                        isValid = image.Max >= _relativeScoreAndImmediateRankCounter.relativeScore;
                }

                if (isValid)
                {
                    if (_activeSprites.ContainsKey(image))
                        continue;
                    var sprite = _imageManager.Spawn(image.SaveData);
                    _activeSprites.Add(image, sprite);
                    sprite.Image = image.Image;
                }
                else
                {
                    if (_activeSprites.TryGetValue(image, out IFSprite sprite))
                    {
                        _activeSprites.Remove(image);
                        _imageManager.Despawn(sprite);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var sprite in _activeSprites)
                _imageManager.Despawn(sprite.Value);
            _relativeScoreAndImmediateRankCounter.relativeScoreOrImmediateRankDidChangeEvent -= RelativeScoreAndImmediateRankCounter_relativeScoreOrImmediateRankDidChangeEvent;
        }

        private class SaveImage
        {
            public Mode Mode { get; }
            public float? Min { get; }
            public float? Max { get; }
            public IFImage Image { get; }
            public IFSaveData SaveData { get; }

            public SaveImage(IFImage image, IFSaveData saveData, Mode mode, float? min = null, float? max = null)
            {
                Min = min;
                Max = max;
                Mode = mode;
                Image = image;
                SaveData = saveData;
            }
        }

        public enum Mode
        {
            Above,
            Below,
            Between
        }
    }
}