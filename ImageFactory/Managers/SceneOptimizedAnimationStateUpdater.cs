using BeatSaberMarkupLanguage.Animations;
using ImageFactory.Interfaces;
using ImageFactory.Models;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Zenject;

namespace ImageFactory.Managers
{
    // Mostly borrowed from BSML's AnimationController
    internal class SceneOptimizedAnimationStateUpdater : IInitializable, ITickable, IDisposable, IAnimationStateUpdater
    {
        private Scene? _initialScene;
        private readonly Dictionary<string, AnimationControllerData> _registeredAnimations = new Dictionary<string, AnimationControllerData>();

        public bool Enabled { get; set; }

        public void Initialize()
        {
            _initialScene = SceneManager.GetActiveScene();
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;    
        }

        private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
        {
            Enabled = _initialScene == newScene;
        }

        public void Dispose()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        public AnimationControllerData Register(string id, ProcessedAnimation processData)
        {
            if (!_registeredAnimations.TryGetValue(id, out AnimationControllerData? animationData))
            {
                animationData = new AnimationControllerData(processData.texture, processData.rect, processData.delays);
                _registeredAnimations.Add(id, animationData);
            }
            else
            {
                UnityEngine.Object.Destroy(processData.texture);
            }
            return animationData;
        }

        public void Unregister(string id)
        {
            if (_registeredAnimations.ContainsKey(id))
                _registeredAnimations.Remove(id);
        }

        public void Tick()
        {
            if (!Enabled)
                return;

            DateTime now = DateTime.UtcNow;
            foreach (AnimationControllerData anim in _registeredAnimations.Values)
                if (anim.IsPlaying)
                    anim.InvokeMethod<object, AnimationControllerData>("CheckFrame", now);
        }
    }
}