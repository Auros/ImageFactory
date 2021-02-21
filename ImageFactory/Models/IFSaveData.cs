using IPA.Config.Stores.Attributes;
using UnityEngine;

namespace ImageFactory.Models
{
    public class IFSaveData
    {
        public virtual bool Enabled { get; set; }
        public virtual Vector2 Size { get; set; }
        public virtual Vector3 Position { get; set; }
        public virtual Quaternion Rotation { get; set; }
        public virtual string Name { get; set; } = null!;
        public virtual string LocalFilePath { get; set; } = null!;

        [NonNullable]
        public virtual ImagePresentationData Presentation { get; set; } = new ImagePresentationData();
    }
}