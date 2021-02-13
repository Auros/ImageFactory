using BeatSaberMarkupLanguage.Animations;
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
            _cancellationTokenSource.Cancel();
        }

        public async Task<IFImage?> LoadAsync(IFImage.Metadata metadata, IAnimationStateUpdater? _ = null)
        {
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
                    Stopwatch watch = Stopwatch.StartNew();
                    Sprite sprite = await _cachedMediaAsyncLoader.LoadSpriteAsync(filePath, _cancellationTokenSource.Token);
                    watch.Stop();

                    image = new IFImage(sprite, metadata, watch.Elapsed);
                }
                else
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    using FileStream imageFS = metadata.file.OpenRead();
                    using MemoryStream imageMS = new MemoryStream();
                    await imageFS.CopyToAsync(imageMS);
                    byte[] imageBytes = imageMS.ToArray();
                    ProcessedAnimation animationData = await Utilities.ProcessAnimation(metadata.animationType.Value, imageBytes);
                    AnimationControllerData data = _animationStateUpdater.Register(filePath, animationData);
                    watch.Stop();

                    image = new IFImage(data, metadata, watch.Elapsed);
                }
                if (!_imageCache.ContainsKey(filePath))
                    _imageCache.Add(filePath, image);

                return image;
            }
            catch (Exception e)
            {
                _siraLog.Error($"Could not load image {filePath}");
                _siraLog.Error(e);
                return null;
            }
        }
    }
}