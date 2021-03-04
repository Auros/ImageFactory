using ImageFactory.Components;
using ImageFactory.Managers;
using System.Linq;
using UnityEngine;
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
            Container.BindInterfacesAndSelfTo<MetadataStore>().AsSingle();
            Container.Bind<DynamicCacheMediaLoader>().AsSingle();
            Container.Bind<ImageEditorManager>().AsSingle();
            Container.Bind<PresentationStore>().AsSingle();

            Container.BindMemoryPool<IFSprite, IFSprite.Pool>().WithInitialSize(5).FromComponentInNewPrefab(ImageTemplate());
        }

        private IFSprite ImageTemplate()
        {
            GameObject root = new GameObject("IF Image");
            IFSprite ifc = root.AddComponent<IFSprite>();
            SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
            ifc.Setup(renderer, BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat, Resources.FindObjectsOfTypeAll<Shader>().First(s => s.name == "Custom/Sprite"));
            root.gameObject.SetActive(false);
            return ifc;
        }
    }
}