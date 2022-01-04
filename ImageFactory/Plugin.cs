using ImageFactory.Installers;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Attributes;
using SiraUtil.Zenject;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

namespace ImageFactory
{
    [Plugin(RuntimeOptions.DynamicInit), Slog, NoEnableDisable]
    public class Plugin
    {
        [Init]
        public Plugin(Conf conf, IPALogger logger, Zenjector zenjector, PluginMetadata metadata)
        {
            Config config = conf.Generated<Config>();
            config.Version = metadata.HVersion;
            zenjector.UseHttpService();
            zenjector.UseLogger(logger);
            zenjector.Install(Location.App, Container =>
            {
                Container.BindInstance(config).AsSingle();
                Container.BindInstance(new UBinder<Plugin, PluginMetadata>(metadata));
            });

            zenjector.Install<IFUIInstaller>(Location.Menu);
            zenjector.Install<IFCoreInstaller>(Location.App);
            zenjector.Install<IFImageInstaller>(Location.App);
            zenjector.Install<IFMenuInstaller>(Location.Menu);
            zenjector.Install<IFGameInstaller>(Location.Player);
        }
    }
}