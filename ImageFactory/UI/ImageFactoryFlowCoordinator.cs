using HMUI;
using BeatSaberMarkupLanguage;
using Zenject;

namespace ImageFactory.UI
{
    internal class ImageFactoryFlowCoordinator : FlowCoordinator
    {
        private IFInfoView _infoView = null!;
        private MainFlowCoordinator _mainFlowCoordinator = null!;

        [Inject]
        public void Inject(IFInfoView infoView, MainFlowCoordinator mainFlowCoordinator)
        {
            _infoView = infoView;
            _mainFlowCoordinator = mainFlowCoordinator;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                // Set our title and back button.
                SetTitle(nameof(ImageFactory));
                showBackButton = true;

                // Upon activating for the first time, let's provide our initial view controllers for this flow coordinator
                // to use. The ScreenSystem needs at a main screen and will break if there is none after activating.
                ProvideInitialViewControllers(_infoView);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            _mainFlowCoordinator.DismissFlowCoordinator(this); // BSML Extension
        }
    }
}