using HMUI;
using BeatSaberMarkupLanguage;
using Zenject;
using ImageFactory.Models;

namespace ImageFactory.UI
{
    internal class ImageFactoryFlowCoordinator : FlowCoordinator
    {
        private Config _config = null!;
        private IFInfoView _infoView = null!;
        private bool _initialEnabledValue = false;
        private IFNewImageView _newImageView = null!;
        private IFEditImageView _editImageView = null!;
        private IFSavedImageView _savedImageView = null!;
        private MainFlowCoordinator _mainFlowCoordinator = null!;
        private MenuTransitionsHelper _menuTransitionsHelper = null!;

        [Inject]
        public void Inject(Config config, IFInfoView infoView, IFNewImageView newImageView, IFEditImageView editImageView, IFSavedImageView savedImageView, MainFlowCoordinator mainFlowCoordinator, MenuTransitionsHelper menuTransitionsHelper)
        {
            _config = config;
            _infoView = infoView;
            _newImageView = newImageView;
            _editImageView = editImageView;
            _savedImageView = savedImageView;
            _mainFlowCoordinator = mainFlowCoordinator;
            _menuTransitionsHelper = menuTransitionsHelper;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            _initialEnabledValue = _config.Enabled;
            if (firstActivation)
            {
                // Set our title and back button.
                SetTitle(nameof(ImageFactory));
                showBackButton = true;

                // Upon activating for the first time, let's provide our initial view controllers for this flow coordinator
                // to use. The ScreenSystem needs at a main screen and will break if there is none after activating.
                ProvideInitialViewControllers(_infoView, _newImageView, _savedImageView);
            }
            _savedImageView.EditImageRequested += SavedImageView_EditImageRequested;
            _newImageView.NewImageRequested += NewImageView_NewImageRequested;
            _editImageView.Cancelled += DismissEditView;
            _editImageView.Saved += DismissEditView;
            _savedImageView.ShouldRefresh = true;
        }

        private void DismissEditView()
        {
            if (_editImageView.isInViewControllerHierarchy)
            {
                ReplaceTopViewController(_infoView, animationType: ViewController.AnimationType.Out);
                SetRightScreenViewController(_savedImageView, ViewController.AnimationType.Out);
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

        private void SavedImageView_EditImageRequested(IFImage image, IFSaveData saveData)
        {
            SetLeftScreenViewController(null, ViewController.AnimationType.Out);
            SetRightScreenViewController(null, ViewController.AnimationType.Out);
            ReplaceTopViewController(_editImageView);
            _editImageView.EnableEditing(image, saveData);
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _editImageView.Saved -= DismissEditView;
            _editImageView.Cancelled -= DismissEditView;
            _newImageView.NewImageRequested -= NewImageView_NewImageRequested;
            _savedImageView.EditImageRequested -= SavedImageView_EditImageRequested;
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
            if (_config.Enabled != _initialEnabledValue)
            {
                _menuTransitionsHelper.RestartGame();
                return;
            }
            _mainFlowCoordinator.DismissFlowCoordinator(this); // BSML Extension
            _savedImageView.ShouldRefresh = false;
        }
    }
}