using BeatSaberMarkupLanguage.Animations;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ImageFactory.Models
{
    // Borred mostly from BSML
    public class RendererAnimationControllerData : AnimationControllerData
    {
        private readonly bool _isDelayConsistent = true;

        public RendererAnimationControllerData(Texture2D tex, Rect[] uvs, float[] delays) : base(tex, uvs, delays)
        {
            float firstDelay = -1;
            for (int i = 0; i < uvs.Length; i++)
            {
                if (i == 0)
                    firstDelay = delays[i];

                if (delays[i] != firstDelay)
                    _isDelayConsistent = false;
            }
        }

        internal void CheckFrame(DateTime now)
        {
            double differenceMs = (now - lastSwitch).TotalMilliseconds;
            if (differenceMs < delays[uvIndex])
                return;

            if (_isDelayConsistent && delays[uvIndex] <= 10 && differenceMs < 100)
            {
                // Bump animations with consistently 10ms or lower frame timings to 100ms
                return;
            }

            lastSwitch = now;
            do
            {
                uvIndex++;
                if (uvIndex >= uvs.Length)
                    uvIndex = 0;
            }
            while (!_isDelayConsistent && delays[uvIndex] == 0);

            if (activeImages.Count != 0)
            {
                foreach (Image image in activeImages)
                {
                    if (image != null && image.isActiveAndEnabled)
                        image.sprite = sprites[uvIndex];
                }
            }
        }
    }
}