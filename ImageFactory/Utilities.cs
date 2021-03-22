using BeatSaberMarkupLanguage.Animations;
using ImageFactory.Models;
using System.Linq;
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

        private static Material _roundEdge = null!;
        public static Material UINoGlowRoundEdge
        {
            get
            {
                if (_roundEdge == null)
                {
                    _roundEdge = Resources.FindObjectsOfTypeAll<Material>().First(m => m.name == "UINoGlowRoundEdge");
                }
                return _roundEdge;
            }
        }

        private static Shader _shader = null!;
        public static Shader ImageShader
        {
            get
            {
                if (_shader == null)
                {
                    _shader = Resources.FindObjectsOfTypeAll<Shader>().First(s => s.name == "Custom/CustomParticles");
                }
                return _shader;
            }
        }
    }
}