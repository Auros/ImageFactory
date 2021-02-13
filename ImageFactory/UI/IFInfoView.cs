using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ImageFactory.Managers;
using IPA.Loader;
using SemVer;
using SiraUtil.Zenject;
using System.Threading;
using System.Threading.Tasks;
using Zenject;

namespace ImageFactory.UI
{
    [ViewDefinition("ImageFactory.Views.info-view.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\info-view.bsml")]
    internal class IFInfoView : BSMLAutomaticViewController
    {
        private Version _version = null!;
        private ISpriteAsyncLoader _spriteAsyncLoader = null!;

        [Inject]
        public void Construct(DynamicCacheMediaLoader dynamicCacheMediaLoader, UBinder<Plugin, PluginMetadata> metadataBinder)
        {
            _version = metadataBinder.Value.Version;
            _spriteAsyncLoader = dynamicCacheMediaLoader;
        }

        [UIComponent("logo-image")]
        protected readonly ImageView _logoImage = null!;

        [UIComponent("settings-image")]
        protected readonly ImageView _settingsImage = null!;

        [UIComponent("reset-image")]
        protected readonly ImageView _resetImage = null!;

        [UIComponent("help-image")]
        protected readonly ImageView _helpImage = null!;

        [UIComponent("github-image")]
        protected readonly ImageView _githubImage = null!;

        [UIComponent("bandoot-image")]
        protected readonly ImageView _bandootImage = null!;

        [UIComponent("auros-image")]
        protected readonly ImageView _aurosImage = null!;

        [UIValue("version")]
        protected string Version => $"v{_version}";
        
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (firstActivation)
            {
                _ = DataLoaderTask();
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

            _settingsImage.mainTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            _resetImage.mainTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            _helpImage.mainTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            _githubImage.mainTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            _bandootImage.mainTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            _aurosImage.mainTexture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
        }
    }
}