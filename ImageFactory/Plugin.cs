using ImageFactory.Installers;
using IPA;
using IPA.Loader;
using SiraUtil;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace ImageFactory
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector, PluginMetadata metadata)
        {
            // Bind our logger and binder separately. It just makes things easier instead
            // of having to pass it as a parameter into our core installer.
            zenjector.On<PCAppInit>().Pseudo(Container =>
            {
                Container.BindLoggerAsSiraLogger(logger);
                Container.BindInstance(new UBinder<Plugin, PluginMetadata>(metadata));
            });
            
            zenjector.OnApp<IFCoreInstaller>();
            zenjector.OnMenu<IFUIInstaller>();
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