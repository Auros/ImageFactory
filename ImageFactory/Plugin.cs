using ImageFactory.Installers;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil;
using SiraUtil.Attributes;
using SiraUtil.Zenject;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace ImageFactory
{
    [Plugin(RuntimeOptions.DynamicInit), Slog]
    public class Plugin
    {
        [Init]
        public Plugin(Conf conf, IPALogger logger, Zenjector zenjector, PluginMetadata metadata)
        {
            Config config = conf.Generated<Config>();
            config.Version = metadata.Version;

            // Bind our logger and binder separately. It just makes things easier instead
            // of having to pass it as a parameter into our core installer.
            zenjector.On<PCAppInit>().Pseudo(Container =>
            {
                Container.BindLoggerAsSiraLogger(logger);
                Container.BindInstance(config).AsSingle();
                Container.BindInstance(new UBinder<Plugin, PluginMetadata>(metadata));
            });
            
            zenjector.OnApp<IFCoreInstaller>();
            zenjector.OnMenu<IFUIInstaller>();

            zenjector.OnMenu<IFImageInstaller>();
            zenjector.OnGame<IFImageInstaller>();
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