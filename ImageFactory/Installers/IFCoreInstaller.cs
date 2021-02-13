using ImageFactory.Managers;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFCoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            // Binding all the interfaces but not the actual type so we don't couple that specific implementation.
            // This allows much more room to change stuff in the future (for example, if we wanted to have a different
            //   sprite loader, all we would have to do is make a new class, follow the implemntation, and change the
            //   type here.)
            // Binding all interfaces also allows us to use the Zenject kernal management interfaces like IInitializable,
            //   IDisposable, and ITickable.

            Container.BindInterfacesTo<CachedIFSpriteLoader>().AsSingle();
            Container.BindInterfacesTo<SimpleAnimationStateUpdater>().AsSingle();
        }
    }
}