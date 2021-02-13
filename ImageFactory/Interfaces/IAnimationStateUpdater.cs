using BeatSaberMarkupLanguage.Animations;
using ImageFactory.Models;

namespace ImageFactory.Interfaces
{
    public interface IAnimationStateUpdater
    {
        /// <summary>
        /// Whether or not to update every animation.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Register animation data into this state updater.
        /// </summary>
        /// <param name="id">The ID to track this animation.</param>
        /// <param name="processData">The raw animation data.</param>
        /// <returns></returns>
        AnimationControllerData Register(string id, ProcessedAnimation processData);

        /// <summary>
        /// Unregister animation data from this state updater.
        /// </summary>
        /// <param name="id">The ID of the animation to remove.</param>
        void Unregister(string id);
    }
}