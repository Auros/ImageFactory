using ImageFactory.Managers;
using ImageFactory.Presenters;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFImageInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ImageManager>().AsSingle();
            Container.BindInterfacesTo<ScenePresenter>().AsSingle();
        }
    }
}