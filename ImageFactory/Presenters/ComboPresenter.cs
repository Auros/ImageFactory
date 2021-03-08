using ImageFactory.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;

namespace ImageFactory.Presenters
{
    internal class ComboPresenter : IInitializable, IDisposable
    {
        private readonly Config _config;
        private readonly SiraLog _siraLog;
        private readonly ImageManager _imageManager;
        private readonly ScoreController _scoreController;
        private readonly List<SaveImage> _savedImages = new List<SaveImage>();
        private readonly Dictionary<SaveImage, IFSprite> _activeSprites = new Dictionary<SaveImage, IFSprite>();

        public const string COMBO_ID = "Combo";
        public const string COMBO_INCREMENT = "Combo Increment";
        public const string COMBO_DROP = "Combo Drop";

        public ComboPresenter(Config config, SiraLog siraLog, ImageManager imageManager, ScoreController scoreController)
        {
            _config = config;
            _siraLog = siraLog;
            _imageManager = imageManager;
            _scoreController = scoreController;
        }

        public async void Initialize()
        {
            var saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID.StartsWith("Combo"));
            foreach (var save in saves)
            {
                IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
                if (metadata.HasValue)
                {
                    var image = await _imageManager.LoadImage(metadata.Value);
                    if (image != null)
                    {
                        bool didParse = false;
                        bool isDrop = false;
                        bool isMod = false;
                        int combo = 0;

                        if (save.Presentation.PresentationID == COMBO_DROP)
                            isDrop = true;
                        else if (int.TryParse(save.Presentation.Value, out combo))
                        {
                            // Putting my foot down. Infinite combo images... are just dumb.
                            if (save.Presentation.Duration == 0)
                                continue;

                            if (save.Presentation.PresentationID == COMBO_INCREMENT)
                                isMod = true;

                            didParse = true;
                        }

                        if (isDrop || didParse)
                        {
                            var saveImage = new SaveImage(image, save, combo, isDrop, isMod);
                            _savedImages.Add(saveImage);
                        }
                    }
                }
            }
            _scoreController.comboDidChangeEvent += ComboChanged;
            _scoreController.comboBreakingEventHappenedEvent += ComboDropped;
        }

        private void ComboChanged(int newCombo)
        {
            foreach (var image in _savedImages)
            {
                if (image.ForDrop)
                    continue;

                bool shouldPresent;
                if (image.Mod)
                    shouldPresent = newCombo % image.Combo == 0;
                else
                    shouldPresent = newCombo == image.Combo;

                if (shouldPresent)
                {
                    Spawn(image);
                }
            }
        }

        private void ComboDropped()
        {
            foreach (var image in _savedImages)
            {
                if (!image.ForDrop)
                    return;

                Spawn(image);
            }
        }

        private void Spawn(SaveImage image)
        {
            if (_activeSprites.ContainsKey(image))
                return;
            var sprite = _imageManager.Spawn(image.SaveData);
            _activeSprites.Add(image, sprite);
            sprite.Image = image.Image;
            _ = Despawn(image);
        }

        private async Task Despawn(SaveImage image)
        {
            int timeInMS = (int)(image.SaveData.Presentation.Duration * 1000f);
            await SiraUtil.Utilities.AwaitSleep(timeInMS);
            if (_activeSprites.TryGetValue(image, out IFSprite sprite))
            {
                _activeSprites.Remove(image);
                _imageManager.Despawn(sprite);
            }
        }

        public void Dispose()
        {
            foreach (var sprite in _activeSprites)
                _imageManager.Despawn(sprite.Value);
            _scoreController.comboBreakingEventHappenedEvent -= ComboDropped;
            _scoreController.comboDidChangeEvent -= ComboChanged;
        }

        private class SaveImage
        {
            public bool Mod { get; }
            public int Combo { get; }
            public bool ForDrop { get; }
            public IFImage Image { get; }
            public IFSaveData SaveData { get; }
        
            public SaveImage(IFImage image, IFSaveData saveData, int combo, bool forDrop, bool mod = false)
            {
                Mod = mod;
                Image = image;
                Combo = combo;
                ForDrop = forDrop;
                SaveData = saveData;
            }
        }
    }
}