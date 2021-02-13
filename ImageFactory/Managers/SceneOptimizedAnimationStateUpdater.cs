using System;
using UnityEngine.SceneManagement;
using Zenject;

namespace ImageFactory.Managers
{
    // Mostly borrowed from BSML's AnimationController
    internal class SceneOptimizedAnimationStateUpdater : SimpleAnimationStateUpdater, IInitializable, IDisposable
    {
        private bool _enabled;
        private Scene? _initialScene;

        public void Initialize()
        {
            _enabled = true;
            _initialScene = SceneManager.GetActiveScene();
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;    
        }

        private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
        {
            // Using a custom _enabled variable instead of the one in the ASU interface for more customizability.
            // Our optimizer should have priority over it. 
            _enabled = _initialScene == newScene;
        }

        public void Dispose()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        public override void Tick()
        {
            if (!_enabled)
                return;

            base.Tick();
        }

        public void SetScene(Scene scene)
        {
            _initialScene = scene;
        }
    }
}