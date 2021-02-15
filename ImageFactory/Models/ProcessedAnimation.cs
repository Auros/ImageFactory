using UnityEngine;

namespace ImageFactory.Models
{
    public struct ProcessedAnimation
    {
        public readonly Texture2D texture;
        public readonly Rect[] rect;
        public readonly float[] delays;
        public readonly int width;
        public readonly int height;

        public ProcessedAnimation(Texture2D texture, Rect[] rect, float[] delays, int width, int height)
        {
            texture.wrapMode = TextureWrapMode.Clamp;
            this.texture = texture;
            this.rect = rect;
            this.delays = delays;
            this.width = width;
            this.height = height;
        }
    }
}