using ImageFactory.Managers;
using ImageFactory.UI;
using SiraUtil;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFUIInstaller : Installer
    {
        public override void InstallBindings()
        {
            // We never actually request the MenuButtonManager anywhere, but it's still created because its interfaces
            // are binded to the Zenject kernal interfaces (IInitializable, IDisposable, etc), so every object that is
            // binded to them will be created anyway, since internally Zenject requests for EVERY kernel interface.
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();

            Container.Bind<IFInfoView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<IFNewImageView>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ImageFactoryFlowCoordinator>().FromNewComponentOnNewGameObject(nameof(ImageFactoryFlowCoordinator)).AsSingle();
        }
    }
}