using ImageFactory.Managers;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFCoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CachedIFSpriteLoader>().AsSingle();
            Container.BindInterfacesTo<SimpleAnimationStateUpdater>().AsSingle();
        }
    }
}