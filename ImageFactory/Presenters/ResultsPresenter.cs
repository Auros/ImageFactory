using ImageFactory.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace ImageFactory.Presenters
{
    internal class ResultsPresenter : IInitializable, IDisposable
    {
        private readonly Config _config;
        private readonly ImageManager _imageManager;
        private readonly ResultsViewController _resultsViewController;
        private readonly List<IFSprite> _spawnedSprites = new List<IFSprite>();
        private bool _allowedToCreate = false;

        private static readonly FieldAccessor<ResultsViewController, LevelCompletionResults>.Accessor Results = FieldAccessor<ResultsViewController, LevelCompletionResults>.GetAccessor("_levelCompletionResults");

        public const string RESULTS_ID = "Results Screen";
        public const string FINISHED_ID = "Finished";
        public const string PASSED_ID = "Passed";
        public const string FAILED_ID = "Failed";

        public ResultsPresenter(Config config, ImageManager imageManager, ResultsViewController resultsViewController)
        {
            _config = config;
            _imageManager = imageManager;
            _resultsViewController = resultsViewController;
        }

        public void Initialize()
        {
            _resultsViewController.didActivateEvent += ResultsViewController_didActivateEvent;
            _resultsViewController.didDeactivateEvent += ResultsViewController_didDeactivateEvent;
        }

        private async void ResultsViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            _allowedToCreate = true;
            var saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == RESULTS_ID);
            foreach (var save in saves)
            {
                var id = save.Presentation.Value;
                var resultsView = _resultsViewController;
                var results = Results(ref resultsView);

                if ((id == PASSED_ID && results.levelEndStateType != LevelCompletionResults.LevelEndStateType.Cleared) || (id == FAILED_ID && results.levelEndStateType != LevelCompletionResults.LevelEndStateType.Failed))
                    continue;

                IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
                if (metadata.HasValue)
                {
                    var image = await _imageManager.LoadImage(metadata.Value);
                    if (image == null)
                        continue;
                    if (!_allowedToCreate)
                        return;
                    var sprite = _imageManager.Spawn(save);
                    _spawnedSprites.Add(sprite);
                    sprite.Image = image;
                }
            }
        }

        private void ResultsViewController_didDeactivateEvent(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _allowedToCreate = false;
            foreach (var sprite in _spawnedSprites)
                _imageManager.Despawn(sprite);
            _spawnedSprites.Clear();
        }

        public void Dispose()
        {
            _resultsViewController.didDeactivateEvent -= ResultsViewController_didDeactivateEvent;
            _resultsViewController.didActivateEvent -= ResultsViewController_didActivateEvent;
        }
    }
}
