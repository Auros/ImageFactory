using System;

namespace ImageFactory.Models
{
    internal class ImageUpdateArgs : EventArgs
    {
        public Action Type { get; }
        public IFImage Image { get; }
        public IFSaveData SaveData { get; }

        public ImageUpdateArgs(Action type, IFImage image, IFSaveData saveData)
        {
            Type = type;
            Image = image;
            SaveData = saveData;
        }
        
        public enum Action
        {
            Added,
            Removed,
            Updated
        }
    }
}