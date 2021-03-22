using ImageFactory.Interfaces;
using ImageFactory.Models;
using SiraUtil.Tools;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ImageFactory.Managers
{
    internal class CachedIFSpriteLoader : IImageFactorySpriteLoader, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly IAnimationStateUpdater _animationStateUpdater;
        private readonly CachedMediaAsyncLoader _cachedMediaAsyncLoader;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Dictionary<string, IFImage> _imageCache = new Dictionary<string, IFImage>();

        public CachedIFSpriteLoader(SiraLog siraLog, IAnimationStateUpdater animationStateUpdater, CachedMediaAsyncLoader cachedMediaAsyncLoader)
        {
            _siraLog = siraLog;
            _animationStateUpdater = animationStateUpdater;
            _cachedMediaAsyncLoader = cachedMediaAsyncLoader;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            // Cancel any operations that might be occuring.
            _cancellationTokenSource.Cancel();
        }

        public async Task<IFImage?> LoadAsync(IFImage.Metadata metadata, IAnimationStateUpdater? animationStateUpdater = null)
        {
            if (metadata.file == null)
                return null;

            string filePath = metadata.file.FullName;
            if (_imageCache.TryGetValue(filePath, out IFImage? image))
            {
                return image;
            }
            try
            {
                _siraLog.Debug($"Loading Sprite: {metadata.file.Name}");
                if (metadata.animationType is null)
                {
                    // Load images through our injected CachedMediaAsyncLoader

                    Stopwatch watch = Stopwatch.StartNew();
                    Sprite sprite = await _cachedMediaAsyncLoader.LoadSpriteAsync(filePath, _cancellationTokenSource.Token);
                    sprite.texture.wrapMode = TextureWrapMode.Clamp;
                    watch.Stop();

                    image = new IFImage(sprite, metadata, watch.Elapsed);
                }
                else
                {
                    // Separate system for loading gifs. Unfortunately we don't have the convenience of the CachedMediaAsyncLoader

                    Stopwatch watch = Stopwatch.StartNew();
                    
                    // Load the file to a byte array
                    using FileStream imageFS = metadata.file.OpenRead();
                    using MemoryStream imageMS = new MemoryStream();
                    await imageFS.CopyToAsync(imageMS);
                    byte[] imageBytes = imageMS.ToArray();

                    // Process the animation data into BSML's animation controller data.
                    ProcessedAnimation animationData = await Utilities.ProcessAnimation(metadata.animationType.Value, imageBytes);
                    RendererAnimationControllerData data = (animationStateUpdater ?? _animationStateUpdater).Register(filePath, animationData);
                    watch.Stop();

                    image = new IFImage(data, metadata, watch.Elapsed);
                }

                // Add it to the cache
                if (!_imageCache.ContainsKey(filePath))
                    _imageCache.Add(filePath, image);

                return image;
            }
            catch (Exception e)
            {
                // It's not actually an image file? Corrupted? Who knows...
                _siraLog.Error($"Could not load image {filePath}");
                _siraLog.Error(e);
                return null;
            }
        }
    }
}