using BeatSaberMarkupLanguage.Animations;
using ImageFactory.Models;
using System.Threading.Tasks;
using UnityEngine;

namespace ImageFactory
{
    internal static class Utilities
    {
        public static Task<ProcessedAnimation> ProcessAnimation(AnimationType type, byte[] data)
        {
            return Task.Run(() =>
            {
                TaskCompletionSource<ProcessedAnimation> source = new TaskCompletionSource<ProcessedAnimation>();
                AnimationLoader.Process(type, data, (Texture2D tex, Rect[] uvs, float[] delays, int width, int height) =>
                {
                    source.SetResult(new ProcessedAnimation(tex, uvs, delays, width, height));
                });
                return source.Task;
            });
        }
    }
}