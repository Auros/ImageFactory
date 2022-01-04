using ImageFactory.Models;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using SiraUtil.Converters;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ImageFactory
{
    internal class Config
    {
        [NonNullable, UseConverter(typeof(VersionConverter))]
        public virtual Hive.Versioning.Version Version { get; set; } = new Hive.Versioning.Version("0.0.0");

        public virtual bool Enabled { get; set; } = true;
        public virtual bool AllowAnimations { get; set; } = true;
        public virtual bool IgnoreTextAndHUDs { get; set; } = false;

        [NonNullable, UseConverter(typeof(ListConverter<IFSaveData>))]
        public virtual List<IFSaveData> SaveData { get; set; } = new List<IFSaveData>();

        public virtual void Changed() { }
        public virtual void CopyFrom(Config _) { }
    }
}