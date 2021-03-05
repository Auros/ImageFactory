using ImageFactory.Interfaces;
using ImageFactory.Models;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;

namespace ImageFactory.Managers
{
    internal class ImageManager : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly List<IFImage> _loadedImages;
        private readonly IImageFactorySpriteLoader _imageFactorySpriteLoader;

        public ImageManager(SiraLog siraLog, IImageFactorySpriteLoader imageFactorySpriteLoader)
        {
            _siraLog = siraLog;
            _loadedImages = new List<IFImage>();
            _imageFactorySpriteLoader = imageFactorySpriteLoader;
        }

        public void Initialize()
        {
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await SiraUtil.Utilities.PauseChamp;
            /*
            _siraLog.Debug("Initializing...");
            Stopwatch watch = Stopwatch.StartNew();
            int count = 0;
            foreach (var metadata in _metadataStore.AllMetadata())
            {
                await LoadImage(metadata);
            }
            watch.Stop();
            _siraLog.Debug($"Took {watch.Elapsed.TotalSeconds} seconds (asynchronous non-blocking) to initialize the Image Factory with {count} active images.");
            */        
        }

        public IEnumerable<IFImage> LoadedImages()
        {
            return _loadedImages;
        }

        public async Task<IFImage?> LoadImage(IFImage.Metadata metadata)
        {
            var cached = _loadedImages.FirstOrDefault(ifi => ifi.metadata.file == metadata.file);
            if (cached != null)
                return cached;

            var image = await _imageFactorySpriteLoader.LoadAsync(metadata);
            if (!(image is null) && !_loadedImages.Any(ifi => ifi.metadata.file == metadata.file))
            {
                _siraLog.Debug($"X: {image.width}px, Y: {image.height}px, Size: {image.metadata.size} bytes, Load Time: {image.loadTime.TotalSeconds}");
                _loadedImages.Add(image);
            }
            return image;
        }

        public void Dispose()
        {

        }
    }
}