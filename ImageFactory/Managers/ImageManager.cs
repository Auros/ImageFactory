using ImageFactory.Interfaces;
using ImageFactory.Models;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Zenject;

namespace ImageFactory.Managers
{
    internal class ImageManager : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly List<IFImage> _loadedImages;
        private readonly MetadataStore _metadataStore;
        private readonly IImageFactorySpriteLoader _imageFactorySpriteLoader;

        public ImageManager(SiraLog siraLog, MetadataStore metadataStore, IImageFactorySpriteLoader imageFactorySpriteLoader)
        {
            _siraLog = siraLog;
            _metadataStore = metadataStore;
            _loadedImages = new List<IFImage>();
            _imageFactorySpriteLoader = imageFactorySpriteLoader;
        }

        public void Initialize()
        {
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _siraLog.Debug("Initializing...");
            Stopwatch watch = Stopwatch.StartNew();
            int count = 0;
            foreach (var metadata in _metadataStore.AllMetadata())
            {
                var image = await _imageFactorySpriteLoader.LoadAsync(metadata);
                if (!(image is null))
                {
                    _siraLog.Debug($"X: {image.width}px, Y: {image.height}px, Size: {image.metadata.size} bytes, Load Time: {image.loadTime.TotalSeconds}");
                    _loadedImages.Add(image);
                    count++;
                }
            }
            watch.Stop();
            _siraLog.Debug($"Took {watch.Elapsed.TotalSeconds} seconds (ASYNC) to initialize the Image Factory with {count} active images.");
        }

        public IEnumerable<IFImage> LoadedImages()
        {
            return _loadedImages;
        }

        public void Dispose()
        {

        }
    }
}