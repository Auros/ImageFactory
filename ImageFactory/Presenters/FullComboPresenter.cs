using ImageFactory.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace ImageFactory.Presenters
{
    internal class FullComboPresenter : IInitializable, IDisposable
    {
        private readonly Config _config;
        private readonly ImageManager _imageManager;
        private readonly IComboController _comboController;
        private readonly List<SaveImage> _savedImages = new List<SaveImage>();
        private readonly Dictionary<SaveImage, IFSprite> _activeSprites = new Dictionary<SaveImage, IFSprite>();

        public const string FULLCOMBO_ID = "Full Combo";

        public FullComboPresenter(Config config, ImageManager imageManager, IComboController comboController)
        {
            _config = config;
            _imageManager = imageManager;
            _comboController = comboController;
        }

        public async void Initialize()
        {
            var saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == FULLCOMBO_ID);
            foreach (var save in saves)
            {
                IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
                if (metadata.HasValue)
                {
                    var image = await _imageManager.LoadImage(metadata.Value);
                    if (image != null)
                    {
                        var saveData = new SaveImage(image, save);
                        _savedImages.Add(saveData);
                        var sprite = _imageManager.Spawn(save);
                        _activeSprites.Add(saveData, sprite);
                        sprite.Image = image;
                    }
                }
            }
            _comboController.comboBreakingEventHappenedEvent += ComboDropped;
        }

        private void ComboDropped()
        {
            foreach (var sprite in _activeSprites)
                _imageManager.Despawn(sprite.Value);
        }

        public void Dispose()
        {
            foreach (var sprite in _activeSprites)
                _imageManager.Despawn(sprite.Value, true);
            _comboController.comboBreakingEventHappenedEvent -= ComboDropped;
        }

        private class SaveImage
        {
            public IFImage Image { get; }
            public IFSaveData SaveData { get; }

            public SaveImage(IFImage image, IFSaveData saveData)
            {
                Image = image;
                SaveData = saveData;
            }
        }
    }
}
