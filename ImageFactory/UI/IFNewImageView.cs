using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
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

        [UIAction("#post-parse")]
        protected void Parsed()
        {
            _selectionCanvas = _selectionRoot.gameObject.AddComponent<CanvasGroup>();
            _loadingCanvas = _loadingRoot.gameObject.AddComponent<CanvasGroup>();
        }
    }
}