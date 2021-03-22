using ImageFactory.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace ImageFactory.Presenters
{
    internal class PausePresenter : IInitializable, IDisposable
    {
        private readonly Config _config;
        private readonly IGamePause _gamePause;
        private readonly ImageManager _imageManager;
        private readonly List<SaveImage> _savedImages = new List<SaveImage>();
        private readonly Dictionary<SaveImage, IFSprite> _activeSprites = new Dictionary<SaveImage, IFSprite>();

        public const string PAUSE_ID = "In Pause Menu";

        public PausePresenter(Config config, IGamePause gamePause, ImageManager imageManager)
        {
            _config = config;
            _gamePause = gamePause;
            _imageManager = imageManager;
        }

        public async void Initialize()
        {
            var saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == PAUSE_ID);
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
                    }
                }
            }
            _gamePause.didPauseEvent += GamePause_didPauseEvent;
            _gamePause.willResumeEvent += GamePause_willResumeEvent;
        }

        private void GamePause_didPauseEvent()
        {
            foreach (var image in _savedImages)
            {
                var sprite = _imageManager.Spawn(image.SaveData);
                _activeSprites.Add(image, sprite);
                sprite.Image = image.Image;
            }
        }

        private void GamePause_willResumeEvent()
        {
            foreach (var image in _savedImages)
            {
                if (_activeSprites.TryGetValue(image, out IFSprite sprite))
                {
                    _activeSprites.Remove(image);
                    _imageManager.Despawn(sprite);
                }
            }
        }

        public void Dispose()
        {
            foreach (var sprite in _activeSprites)
                _imageManager.Despawn(sprite.Value, true);
            _gamePause.willResumeEvent -= GamePause_willResumeEvent;
            _gamePause.didPauseEvent -= GamePause_didPauseEvent;
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