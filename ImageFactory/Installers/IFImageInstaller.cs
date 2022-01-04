using ImageFactory.Presenters;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFImageInstaller : Installer
    {
        private readonly Config _config;

        public IFImageInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            if (!_config.Enabled)
                return;

            Container.BindInterfacesTo<ScenePresenter>().AsSingle();
        }
    }
}