using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using ImageFactory.UI;
using System;
using Zenject;

namespace ImageFactory.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;
        private readonly MainFlowCoordinator _mainFlowCoordinator;
        private readonly ImageFactoryFlowCoordinator _imageFactoryFlowCoordinator;

        public MenuButtonManager(MainFlowCoordinator mainFlowCoordinator, ImageFactoryFlowCoordinator imageFactoryFlowCoordinator)
        {
            _menuButton = new MenuButton(nameof(ImageFactory), ShowFlowCoordinator);
            _mainFlowCoordinator = mainFlowCoordinator;
            _imageFactoryFlowCoordinator = imageFactoryFlowCoordinator;
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            // Because of the way singletons work, without this if statement they might
            // be destroyed by the time the menu button manager is disposing.
            // PersistentSingletons are WeirdChamp.
            if (BSMLParser.IsSingletonAvailable && MenuButtons.IsSingletonAvailable)
                MenuButtons.instance.UnregisterButton(_menuButton);
        }

        private void ShowFlowCoordinator()
        {
            _mainFlowCoordinator.PresentFlowCoordinator(_imageFactoryFlowCoordinator, animationDirection: HMUI.ViewController.AnimationDirection.Vertical);
        }
    }
}