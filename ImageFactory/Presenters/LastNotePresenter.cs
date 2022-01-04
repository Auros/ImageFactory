using ImageFactory.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;

namespace ImageFactory.Presenters
{
    internal class LastNotePresenter : IInitializable, IDisposable
    {
        private bool _didLastNote = false;
        private float _lastNoteTime = 0.0f;
        public const string LASTNOTE_ID = "On Last Note";

        private readonly Config _config;
        private readonly ImageManager _imageManager;
        private readonly IDifficultyBeatmap _difficultyBeatmap;
        private readonly BeatmapObjectManager _beatmapObjectManager;
        private readonly List<SaveImage> _savedImages = new List<SaveImage>();
        private readonly Dictionary<SaveImage, IFSprite> _activeSprites = new Dictionary<SaveImage, IFSprite>();

        public LastNotePresenter(Config config, ImageManager imageManager, IDifficultyBeatmap difficultyBeatmap, BeatmapObjectManager beatmapObjectManager)
        {
            _config = config;
            _imageManager = imageManager;
            _difficultyBeatmap = difficultyBeatmap;
            _beatmapObjectManager = beatmapObjectManager;
        }

        public async void Initialize()
        {
            foreach (var line in _difficultyBeatmap.beatmapData.beatmapLinesData)
                foreach (var data in line.beatmapObjectsData)
                    if (data.beatmapObjectType == BeatmapObjectType.Note && data is NoteData note && note.colorType != ColorType.None)
                        if (data.time > _lastNoteTime)
                            _lastNoteTime = data.time;

            _lastNoteTime -= 0.01f;
            var saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == LASTNOTE_ID);
            foreach (var save in saves)
            {
                if (save.Presentation.Duration == 0)
                    continue;

                IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
                if (metadata.HasValue)
                {
                    var image = await _imageManager.LoadImage(metadata.Value);
                    if (image != null)
                    {
                        var saveImage = new SaveImage(image, save);
                        _savedImages.Add(saveImage);
                    }
                }
            }

            _beatmapObjectManager.noteWasCutEvent += BeatmapObjectManager_noteWasCutEvent;
            _beatmapObjectManager.noteWasMissedEvent += BeatmapObjectManager_noteWasMissedEvent;
        }

        private void BeatmapObjectManager_noteWasMissedEvent(NoteController noteController)
        {
            CheckLastNote(noteController.noteData.time);
        }

        private void BeatmapObjectManager_noteWasCutEvent(NoteController noteController, in NoteCutInfo _)
        {
            CheckLastNote(noteController.noteData.time);
        }

        private void CheckLastNote(float currentNoteTime)
        {
            if (_didLastNote)
                return;

            if (currentNoteTime >= _lastNoteTime)
            {
                _didLastNote = true;
                foreach (var image in _savedImages)
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
            await Task.Delay(timeInMS);
            if (_activeSprites.TryGetValue(image, out IFSprite sprite))
            {
                _activeSprites.Remove(image);
                _imageManager.Despawn(sprite);
            }
        }

        public void Dispose()
        {
            foreach (var sprite in _activeSprites)
                _imageManager.Despawn(sprite.Value, true);
            _beatmapObjectManager.noteWasCutEvent -= BeatmapObjectManager_noteWasCutEvent;
            _beatmapObjectManager.noteWasMissedEvent -= BeatmapObjectManager_noteWasMissedEvent;
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