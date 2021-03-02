using HMUI;
using BeatSaberMarkupLanguage;
using Zenject;
using ImageFactory.Models;

namespace ImageFactory.UI
{
    internal class ImageFactoryFlowCoordinator : FlowCoordinator
    {
        private IFInfoView _infoView = null!;
        private IFNewImageView _newImageView = null!;
        private MainFlowCoordinator _mainFlowCoordinator = null!;

        [Inject]
        public void Inject(IFInfoView infoView, IFNewImageView newImageView, MainFlowCoordinator mainFlowCoordinator)
        {
            _infoView = infoView;
            _newImageView = newImageView;
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
                ProvideInitialViewControllers(_infoView, _newImageView);
            }
            _newImageView.NewImageRequested += NewImageView_NewImageRequested;
        }

        private void NewImageView_NewImageRequested(IFImage image)
        {

        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _newImageView.NewImageRequested -= NewImageView_NewImageRequested;
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            _mainFlowCoordinator.DismissFlowCoordinator(this); // BSML Extension
        }
    }
}