using ImageFactory.Interfaces;
using ImageFactory.Models;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using Zenject;

namespace ImageFactory.Managers
{
    // Mostly borrowed from BSML's AnimationController
    internal class SimpleAnimationStateUpdater : ITickable, IAnimationStateUpdater
    {
        private readonly Dictionary<string, RendererAnimationControllerData> _registeredAnimations = new Dictionary<string, RendererAnimationControllerData>();

        public bool Enabled { get; set; } = true;

        public RendererAnimationControllerData Register(string id, ProcessedAnimation processData)
        {
            if (!_registeredAnimations.TryGetValue(id, out RendererAnimationControllerData? animationData))
            {
                // Add the new data to our registration.
                animationData = new RendererAnimationControllerData(processData.texture, processData.rect, processData.delays);
                _registeredAnimations.Add(id, animationData);
            }
            else
            {
                // Destroy the extra texture to free up RAM if
                // a synchronization issue occurs.
                UnityEngine.Object.Destroy(processData.texture);
            }
            return animationData;
        }

        public void Unregister(string id)
        {
            if (_registeredAnimations.ContainsKey(id))
                _registeredAnimations.Remove(id);
        }

        public virtual void Tick()
        {
            if (!Enabled)
                return;

            DateTime now = DateTime.UtcNow;

            // For every animation controller we have, update the images under its effect.
            foreach (RendererAnimationControllerData anim in _registeredAnimations.Values)
                if (anim.IsPlaying)
                    anim.InvokeMethod<object, RendererAnimationControllerData>("CheckFrame", now);
        }
    }
}