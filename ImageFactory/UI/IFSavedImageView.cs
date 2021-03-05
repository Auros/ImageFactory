using BeatSaberMarkupLanguage.Animations;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ImageFactory.Managers;
using ImageFactory.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ImageFactory.UI
{
    [HotReload(RelativePathToLayout = @"..\Views\saved-image-view.bsml")]
    [ViewDefinition("ImageFactory.Views.saved-image-view.bsml")]
    internal class IFSavedImageView : BSMLAutomaticViewController
    {
        public event Action<IFImage, IFSaveData>? EditImageRequested;

        #region Injected Dependencies

        protected TweeningManager _tweeningManager = null!;

        protected MetadataStore _metadataStore = null!;

        protected ImageManager _imageManager = null!;

        protected Config _config = null!;

        [Inject]
        protected void Construct(Config config, DiContainer container, ImageManager imageManager, MetadataStore metadataStore, TweeningManager tweeningManager)
        {
            _tweeningManager = tweeningManager;
            _metadataStore = metadataStore;
            _imageManager = imageManager;
            _config = config;
        }

        #endregion

        #region Canvas Animator

        [UIComponent("selection-root")]
        protected readonly RectTransform _selectionRoot = null!;
        protected CanvasGroup _selectionCanvas = null!;

        [UIComponent("loading-root")]
        protected readonly RectTransform _loadingRoot = null!;
        protected CanvasGroup _loadingCanvas = null!;

        public async Task AnimateToSelectionCanvas()
        {
            const float animationSector = 0.4f;

            _loadingCanvas.alpha = 1f;
            _loadingCanvas.gameObject.SetActive(true);
            _tweeningManager.KillAllTweens(_loadingCanvas);
            _tweeningManager.AddTween(new FloatTween(1f, 0f, val =>
            {
                _loadingCanvas.alpha = val;
            }, animationSector, EaseType.InOutQuad), _loadingCanvas);

            await SiraUtil.Utilities.AwaitSleep((int)(animationSector * 1000));
            _loadingCanvas.gameObject.SetActive(false);

            _selectionCanvas.alpha = 0f;
            _selectionCanvas.gameObject.SetActive(true);
            _tweeningManager.KillAllTweens(_selectionCanvas);
            _tweeningManager.AddTween(new FloatTween(0f, 1f, val =>
            {
                _selectionCanvas.alpha = val;
            }, animationSector, EaseType.InOutQuad), _selectionCanvas);
        }

        #endregion

        #region Image Loader

        [UIComponent("image-list")]
        protected readonly CustomCellListTableData _imageList = null!;
        private bool _didLoad = false;

        [UIComponent("up-button")]
        protected readonly Button _upbutton = null!;

        [UIComponent("down-button")]
        protected readonly Button _downButton = null!;

        public async Task LoadImages()
        {
            if (_didLoad)
                return;

            foreach (var metadata in _metadataStore.AllMetadata())
                await _imageManager.LoadImage(metadata);

            var loadedImages = _imageManager.LoadedImages();
            await AnimateToSelectionCanvas();
            _didLoad = true;

            foreach (var save in _config.SaveData)
            {
                var image = loadedImages.FirstOrDefault(i => i.metadata.file.Name == save.LocalFilePath);
                if (image != null)
                    _imageList.data.Add(new EditImageCell(image, save, ClickedImageEdit));
            }
            _imageList.tableView.ReloadData();
        }

        private void ClickedImageEdit(IFImage image, IFSaveData saveData)
        {
            EditImageRequested?.Invoke(image, saveData);
        }

        #endregion

        [UIAction("#post-parse")]
        protected void Parsed()
        {
            _selectionCanvas = _selectionRoot.gameObject.AddComponent<CanvasGroup>();
            _loadingCanvas = _loadingRoot.gameObject.AddComponent<CanvasGroup>();
            //_imageList.tableView.SetButtons(_upbutton, _downButton);
            _ = LoadImages();
        }

        private class EditImageCell
        {
            public readonly IFImage image;
            public readonly IFSaveData saveData;
            public readonly Action<IFImage, IFSaveData> editAction;

            [UIComponent("preview")]
            protected readonly ImageView _previewImage = null!;

            [UIComponent("file-name")]
            protected readonly CurvedTextMeshPro _fileName = null!;

            public EditImageCell(IFImage image, IFSaveData saveData, Action<IFImage, IFSaveData> editClicked)
            {
                this.image = image;
                this.saveData = saveData;
                editAction = editClicked;
            }

            [UIAction("#post-parse")]
            protected void Parsed()
            {
                _previewImage.sprite = image.sprite;
                if (image.animationData != null)
                {
                    var stateUpdater = _previewImage.gameObject.AddComponent<AnimationStateUpdater>();
                    image.animationData.activeImages.Add(_previewImage);
                    stateUpdater.image = _previewImage;
                }
                else
                {
                    _previewImage.material = Utilities.UINoGlowRoundEdge;
                }
                _fileName.text = saveData.Name;
            }

            [UIAction("clicked-edit-button")]
            protected void ClickedEditButton()
            {
                editAction?.Invoke(image, saveData);
            }
        }
    }
}