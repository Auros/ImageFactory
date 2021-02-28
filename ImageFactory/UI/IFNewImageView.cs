using BeatSaberMarkupLanguage.Animations;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ImageFactory.Managers;
using ImageFactory.Models;
using System.Threading.Tasks;
using Tweening;
using UnityEngine;
using Zenject;

namespace ImageFactory.UI
{
    [HotReload(RelativePathToLayout = @"..\Views\new-image-view.bsml")]
    [ViewDefinition("ImageFactory.Views.new-image-view.bsml")]
    internal class IFNewImageView : BSMLAutomaticViewController
    {
        //public event Action<IFImage>? NewImageRequested;

        #region Injected Dependencies

        [Inject]
        protected readonly TweeningManager _tweeningManager = null!;

        [Inject]
        protected readonly MetadataStore _metadataStore = null!;

        [Inject]
        protected readonly ImageManager _imageManager = null!;

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
        
        public async Task LoadImages()
        {
            if (_didLoad)
                return;

            foreach (var metadata in _metadataStore.AllMetadata())
                await _imageManager.LoadImage(metadata);

            var loadedImages = _imageManager.LoadedImages();
            await AnimateToSelectionCanvas();
            _didLoad = true;

            foreach (var image in loadedImages)
                _imageList.data.Add(new NewImageCell(image));
            _imageList.tableView.ReloadData();
        }

        #endregion

        [UIAction("#post-parse")]
        protected void Parsed()
        {
            _selectionCanvas = _selectionRoot.gameObject.AddComponent<CanvasGroup>();
            _loadingCanvas = _loadingRoot.gameObject.AddComponent<CanvasGroup>();
            _ = LoadImages();
        }

        private class NewImageCell
        {
            public readonly IFImage image;

            [UIComponent("preview")]
            protected readonly ImageView _previewImage = null!;

            [UIComponent("file-name")]
            protected readonly CurvedTextMeshPro _fileName = null!;

            public NewImageCell(IFImage image)
            {
                this.image = image;
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
                _fileName.text = image.metadata.file.Name;
            }
        }
    }
}