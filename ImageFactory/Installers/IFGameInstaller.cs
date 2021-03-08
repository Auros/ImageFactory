using ImageFactory.Presenters;
using Zenject;

namespace ImageFactory.Installers
{
    internal class IFGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LastNotePresenter>().AsSingle();
            Container.BindInterfacesTo<PercentPresenter>().AsSingle();
            Container.BindInterfacesTo<ComboPresenter>().AsSingle();
        }
    }
}