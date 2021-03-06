using ImageFactory.Components;
using ImageFactory.Interfaces;
using ImageFactory.Models;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageFactory.Managers
{
    internal class ImageManager : IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly List<IFImage> _loadedImages;
        private readonly MonoMemoryPoolContainer<IFSprite> _spritePool;
        private readonly IImageFactorySpriteLoader _imageFactorySpriteLoader;
        private readonly List<IFSprite> _recentlyDeanimated = new List<IFSprite>();

        public event EventHandler<ImageUpdateArgs>? ImageUpdated;

        public ImageManager(SiraLog siraLog, IFSprite.Pool spritePool, IImageFactorySpriteLoader imageFactorySpriteLoader)
        {
            _siraLog = siraLog;
            _loadedImages = new List<IFImage>();
            _imageFactorySpriteLoader = imageFactorySpriteLoader;
            _spritePool = new MonoMemoryPoolContainer<IFSprite>(spritePool);
        }

        public IFSprite Spawn(IFSaveData data)
        {
            var sprite = _spritePool.Spawn();
            sprite.Position = data.Position;
            sprite.Rotation = data.Rotation;
            sprite.Size = data.Size;
            return sprite;
        }

        public void Despawn(IFSprite sprite)
        {
            _spritePool.Despawn(sprite);
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
                //_siraLog.Debug($"X: {image.width}px, Y: {image.height}px, Size: {image.metadata.size} bytes, Load Time: {image.loadTime.TotalSeconds}");
                _loadedImages.Add(image);
            }
            return image;
        }

        public void UpdateImage(IFImage image, IFSaveData saveData, ImageUpdateArgs.Action action = ImageUpdateArgs.Action.Updated)
        {
            ImageUpdated?.Invoke(this, new ImageUpdateArgs(action, image, saveData));
        }

        public void ReanimateAll()
        {
            foreach (var sprite in _recentlyDeanimated)
                sprite.AnimateIn();
            _recentlyDeanimated.Clear();
        }

        public void DeanimateAll()
        {
            _recentlyDeanimated.Clear();
            _recentlyDeanimated.AddRange(_spritePool.activeItems);
            foreach (var sprite in _recentlyDeanimated)
                sprite.AnimateOut();
        }

        public void Dispose()
        {

        }
    }
}