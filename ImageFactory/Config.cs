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
        public virtual SemVer.Version Version { get; set; } = new SemVer.Version("0.0.0");

        [NonNullable, UseConverter(typeof(ListConverter<IFSaveData>))]
        public virtual List<IFSaveData> SaveData { get; set; } = new List<IFSaveData>();
    }
}