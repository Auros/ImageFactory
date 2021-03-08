using ImageFactory.Presenters;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFGameInstaller : Installer
    {
        private readonly Config _config;
        private readonly GameplayCoreSceneSetupData _sceneSetupData;

        public IFGameInstaller(Config config, GameplayCoreSceneSetupData sceneSetupData)
        {
            _config = config;
            _sceneSetupData = sceneSetupData;
        }

        public override void InstallBindings()
        {
            //if (!_config.IgnoreTextAndHUDs && _sceneSetupData.playerSpecificSettings.noTextsAndHuds)
            //    return;

            Container.BindInterfacesTo<LastNotePresenter>().AsSingle();
            Container.BindInterfacesTo<PercentPresenter>().AsSingle();
            Container.BindInterfacesTo<ComboPresenter>().AsSingle();
        }
    }
}