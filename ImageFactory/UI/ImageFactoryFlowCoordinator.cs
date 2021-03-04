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
        private IFEditImageView _editImageView = null!;
        private MainFlowCoordinator _mainFlowCoordinator = null!;

        [Inject]
        public void Inject(IFInfoView infoView, IFNewImageView newImageView, IFEditImageView editImageView, MainFlowCoordinator mainFlowCoordinator)
        {
            _infoView = infoView;
            _newImageView = newImageView;
            _editImageView = editImageView;
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
            _editImageView.Cancelled += DismissEditView;
            _editImageView.Saved += DismissEditView;
        }

        private void DismissEditView()
        {
            if (_editImageView.isInViewControllerHierarchy)
            {
                ReplaceTopViewController(_infoView, animationType: ViewController.AnimationType.Out);
                SetLeftScreenViewController(_newImageView, ViewController.AnimationType.Out);
            }
        }

        private void NewImageView_NewImageRequested(IFImage image)
        {
            SetLeftScreenViewController(null, ViewController.AnimationType.Out);
            SetRightScreenViewController(null, ViewController.AnimationType.Out);
            ReplaceTopViewController(_editImageView);
            _editImageView.EnableEditing(image);
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _editImageView.Saved -= DismissEditView;
            _editImageView.Cancelled -= DismissEditView;
            _newImageView.NewImageRequested -= NewImageView_NewImageRequested;
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            // To prevent confusion, if they are currently editing
            // an image, send them back (from where) to our main menu.
            if (_editImageView.isInViewControllerHierarchy)
            {
                DismissEditView();
                return;
            }
            _mainFlowCoordinator.DismissFlowCoordinator(this); // BSML Extension
        }
    }
}