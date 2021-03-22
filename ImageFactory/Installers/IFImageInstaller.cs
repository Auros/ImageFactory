using ImageFactory.Managers;
using ImageFactory.Presenters;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFImageInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ScenePresenter>().AsSingle();
        }
    }
}