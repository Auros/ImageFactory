using ImageFactory.Managers;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFImageInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ImageManager>().AsSingle();

            Container.BindInterfacesAndSelfTo<TestStuff>().AsSingle();
        }
    }
}