using BeatSaberMarkupLanguage.Animations;
using ImageFactory.Models;

namespace ImageFactory.Interfaces
{
    public interface IAnimationStateUpdater
    {
        bool Enabled { get; set; }
        AnimationControllerData Register(string id, ProcessedAnimation processData);
        void Unregister(string id);
    }
}