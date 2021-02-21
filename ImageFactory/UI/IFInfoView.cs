using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ImageFactory.Managers;
using IPA.Loader;
using IPA.Utilities;
using SemVer;
using SiraUtil.Zenject;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ImageFactory.UI
{
    [ViewDefinition("ImageFactory.Views.info-view.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\info-view.bsml")]
    internal class IFInfoView : BSMLAutomaticViewController
    {
        private Version _version = null!;
        private ClickableSubIcon? _lastSubIcon;
        private ISpriteAsyncLoader _spriteAsyncLoader = null!;
        internal static readonly FieldAccessor<ImageView, float>.Accessor ImageSkew = FieldAccessor<ImageView, float>.GetAccessor("_skew");

        [Inject]
        public void Construct(DynamicCacheMediaLoader dynamicCacheMediaLoader, UBinder<Plugin, PluginMetadata> metadataBinder)
        {
            _version = metadataBinder.Value.Version;
            _spriteAsyncLoader = dynamicCacheMediaLoader;
        }

        [UIParams]
        protected readonly BSMLParserParams parserParams = null!;

        [UIComponent("logo-image")]
        protected readonly ImageView _logoImage = null!;

        [UIComponent("settings-image")]
        protected readonly ImageView _settingsImage = null!;

        [UIComponent("reset-image")]
        protected readonly ImageView _resetImage = null!;
        private ClickableSubIcon _resetSubIcon = null!;

        [UIComponent("help-image")]
        protected readonly ImageView _helpImage = null!;
        private ClickableSubIcon _helpSubIcon = null!;

        [UIComponent("github-image")]
        protected readonly ImageView _githubImage = null!;
        private ClickableSubIcon _githubSubIcon = null!;

        [UIComponent("bandoot-image")]
        protected readonly ImageView _bandootImage = null!;
        private ClickableSubIcon _bandootSubIcon = null!;

        [UIComponent("auros-image")]
        protected readonly ImageView _aurosImage = null!;
        private ClickableSubIcon _aurosSubIcon = null!;

        [UIValue("version")]
        protected string Version => $"v{_version}";

        [UIComponent("dismiss-button")]
        protected readonly Button _dismissButton = null!;

        [UIComponent("action-button")]
        protected readonly Button _actionButton = null!;

        [UIComponent("modal-image")]
        protected readonly ImageView _modalImage = null!;

        private string _modalTitle = "";
        [UIValue("modal-title")]
        protected string ModalTitle
        {
            get => _modalTitle;
            set { _modalTitle = value; NotifyPropertyChanged(); }
        }

        private string _modalText = "";
        [UIValue("modal-text")]
        protected string ModalText
        {
            get => _modalText;
            set { _modalText = value; NotifyPropertyChanged(); }
        }

        private string _modalActionText = "";
        [UIValue("modal-action-text")]
        protected string ModalActionText
        {
            get => _modalActionText;
            set { _modalActionText = value; NotifyPropertyChanged(); }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (firstActivation)
            {
                _ = DataLoaderTask();

                Button[] buttons = new Button[2] { _dismissButton, _actionButton };
                foreach (var button in buttons)
                {
                    foreach (var imageView in button.GetComponentsInChildren<ImageView>())
                    {
                        var image = imageView;
                        ImageSkew(ref image) = 0f;
                    }
                }
            }
        }

        // Loads all the various stuff as asynchronously as possible.
        // There are easier ways to get stuff like images on BSML like using
        // the raw assembly location as the parameter and letting BSML do the
        // parsing, but I like to be ASAF (a-syncronous as fuck)
        protected async Task DataLoaderTask()
        {
            _logoImage.sprite = await _spriteAsyncLoader.LoadSpriteAsync("logo.png", CancellationToken.None);
            _settingsImage.sprite = await _spriteAsyncLoader.LoadSpriteAsync("settings.png", CancellationToken.None);
            _resetImage.sprite = await _spriteAsyncLoader.LoadSpriteAsync("reset.png", CancellationToken.None);
            _helpImage.sprite = await _spriteAsyncLoader.LoadSpriteAsync("help.png", CancellationToken.None);
            _githubImage.sprite = await _spriteAsyncLoader.LoadSpriteAsync("https://cdn.sira.pro/images/common/github.png", CancellationToken.None);
            _bandootImage.sprite = await _spriteAsyncLoader.LoadSpriteAsync("https://cdn.sira.pro/images/common/bandoot.png", CancellationToken.None);
            _aurosImage.sprite = await _spriteAsyncLoader.LoadSpriteAsync("https://cdn.sira.pro/images/common/auros.png", CancellationToken.None);
            
            _settingsImage.material = Utilities.UINoGlowRoundEdge;
            _resetImage.material = Utilities.UINoGlowRoundEdge;
            _helpImage.material = Utilities.UINoGlowRoundEdge;
            _githubImage.material = Utilities.UINoGlowRoundEdge;
            _bandootImage.material = Utilities.UINoGlowRoundEdge;
            _aurosImage.material = Utilities.UINoGlowRoundEdge;
            _modalImage.material = Utilities.UINoGlowRoundEdge;

            _settingsImage.mainTexture.wrapMode = TextureWrapMode.Clamp;
            _resetImage.mainTexture.wrapMode = TextureWrapMode.Clamp;
            _helpImage.mainTexture.wrapMode = TextureWrapMode.Clamp;
            _githubImage.mainTexture.wrapMode = TextureWrapMode.Clamp;
            _bandootImage.mainTexture.wrapMode = TextureWrapMode.Clamp;
            _aurosImage.mainTexture.wrapMode = TextureWrapMode.Clamp;

            _resetSubIcon = new ClickableSubIcon(
                "Reset",
                "Note: This is a permanent action that can't be undone. This will reset all of your settings. (Your images will not be deleted).",
                "<color=red>Reset</color>",
                _resetImage.sprite,
                () => { }); // TODO: Reset Config

            _helpSubIcon = new ClickableSubIcon(
                "Help and FAQ",
                "ImageFactory allows you to place custom images throughout your game. Everything has been designed to be easy for the user to use and customizable. To learn more about how to use this mod, you can start the <color=#dbb716>tutorial</color>.",
                "<color=#dbb716>Start Tutorial</color>",
                _helpImage.sprite,
                () => { }); // TODO: Start the tutorial

            _githubSubIcon = new ClickableSubIcon(
                "Github",
                "ImageFactory is open source! You can view it on GitHub. Find a bug report or have a feature request? Submit an issue on GitHub.",
                "Open Github in Browser",
                _githubImage.sprite,
                () => { Application.OpenURL("https://github.com/Auros/ImageFactory"); }); 

            _bandootSubIcon = new ClickableSubIcon(
                "Bandoot",
                "Bandoot commissioned for this mod to be made, go check him out!",
                "Opens Twitch in Browser",
                _bandootImage.sprite,
                () => { Application.OpenURL("https://twitch.tv/bandoot"); });

            _aurosSubIcon = new ClickableSubIcon(
                "Auros",
                "ImageFactory was made by me (Auros). If you'd like to support me, you can go to my <color=green>donation</color> page below.",
                "Opens <color=green>Ko-Fi</color> in Browser",
                _aurosImage.sprite,
                () => { Application.OpenURL("https://ko-fi.com/aurosnex"); }); 
        }

        private void PresentSubIcon(ClickableSubIcon icon)
        {
            _lastSubIcon = icon;
            _modalImage.sprite = _lastSubIcon.sprite;
            ModalActionText = _lastSubIcon.actionText;
            ModalTitle = _lastSubIcon.title;
            ModalText = _lastSubIcon.text;

            parserParams.EmitEvent("show-modal");
        }

        [UIAction("modal-action")]
        protected void ModalActionClicked()
        {
            _lastSubIcon?.onClick?.Invoke();
        }

        [UIAction("clicked-reset")] protected void ClickedReset() => PresentSubIcon(_resetSubIcon);
        [UIAction("clicked-help")] protected void ClickedHelp() => PresentSubIcon(_helpSubIcon);
        [UIAction("clicked-github")] protected void ClickedGitHub() => PresentSubIcon(_githubSubIcon);
        [UIAction("clicked-bandoot")] protected void ClickedBandoot() => PresentSubIcon(_bandootSubIcon);
        [UIAction("clicked-auros")] protected void ClickedAuros() => PresentSubIcon(_aurosSubIcon);

        private class ClickableSubIcon
        {
            public readonly string title;
            public readonly string text;
            public readonly string actionText;
            public readonly Sprite sprite;
            public readonly System.Action onClick;
            public ClickableSubIcon(string title, string text, string actionText, Sprite sprite, System.Action onClick)
            {
                this.title = title;
                this.text = text;
                this.actionText = actionText;
                this.sprite = sprite;
                this.onClick = onClick;
            }
        }
    }
}