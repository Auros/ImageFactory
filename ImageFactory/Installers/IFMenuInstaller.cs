using ImageFactory.Presenters;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFMenuInstaller : Installer
    {
        private readonly Config _config;

        public IFMenuInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            if (!_config.Enabled)
                return;

            Container.BindInterfacesTo<ResultsPresenter>().AsSingle();
        }
    }
}