using BeatSaberMarkupLanguage.Animations;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ImageFactory.Components
{
    internal class RendererAnimationStateUpdater : MonoBehaviour
    {
        public SpriteRenderer? Renderer { get; set; }
        public AnimationControllerData? ControllerData { get; set; }

        protected void LateUpdate()
        {
            if (Renderer != null && ControllerData != null)
            {
                Renderer.sprite = ControllerData.sprites[ControllerData.uvIndex];
            }
        }
    }
}