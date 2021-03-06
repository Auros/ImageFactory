using ImageFactory.Presenters;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ResultsPresenter>().AsSingle();
        }
    }
}