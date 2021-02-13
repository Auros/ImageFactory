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
    internal class SceneOptimizedAnimationStateUpdater : SimpleAnimationStateUpdater, IInitializable, IDisposable
    {
        private Scene? _initialScene;

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
    }
}