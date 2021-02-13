using ImageFactory.Installers;
using IPA;
using SiraUtil;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace ImageFactory
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            zenjector.On<PCAppInit>().Pseudo(Container => Container.BindLoggerAsSiraLogger(logger));
            zenjector.OnApp<IFCoreInstaller>();
        }

        [OnEnable]
        public void OnEnable()
        {

        }

        [OnDisable]
        public void OnDisable()
        {

        }
    }
}