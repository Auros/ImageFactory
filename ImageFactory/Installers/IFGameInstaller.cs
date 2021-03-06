using ImageFactory.Presenters;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PercentPresenter>().AsSingle();
        }
    }
}