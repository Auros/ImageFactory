using ImageFactory.Interfaces;
using SiraUtil.Tools;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Zenject;

namespace ImageFactory.Managers
{
    internal class ImageManager : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly MetadataStore _metadataStore;
        private readonly IImageFactorySpriteLoader _imageFactorySpriteLoader;

        public ImageManager(SiraLog siraLog, MetadataStore metadataStore, IImageFactorySpriteLoader imageFactorySpriteLoader)
        {
            _siraLog = siraLog;
            _metadataStore = metadataStore;
            _imageFactorySpriteLoader = imageFactorySpriteLoader;
        }

        public void Initialize()
        {
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await SiraUtil.Utilities.AwaitSleep(5000);
            _siraLog.Debug("Initializing...");
            Stopwatch watch = Stopwatch.StartNew();
            int count = 0;
            foreach (var metadata in _metadataStore.AllMetadata())
            {
                var image = await _imageFactorySpriteLoader.LoadAsync(metadata);
                if (!(image is null))
                {
                    _siraLog.Debug($"X: {image.width}px, Y: {image.height}px, Size: {image.metadata.size} bytes, Load Time: {image.loadTime.TotalSeconds}");
                    count++;
                }
            }
            watch.Stop();
            _siraLog.Debug($"Took {watch.Elapsed.TotalSeconds} seconds (ASYNC) to initialize the Image Factory with {count} active images.");
        }

        public void Dispose()
        {

        }
    }
}