using ImageFactory.Models;
using IPA.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zenject;

namespace ImageFactory.Managers
{
    internal class MetadataStore : IInitializable
    {
        private readonly DirectoryInfo _saveDirectory = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, nameof(ImageFactory), "Images"));
        private readonly Dictionary<string, IFImage.Metadata> _metadataStore = new Dictionary<string, IFImage.Metadata>();
        private static readonly string[] AllowedExtensions = new string[] { ".png", ".apng", ".gif", ".jpg", ".jpeg" };

        public void Initialize()
        {
            if (!_saveDirectory.Exists)
            {
                _saveDirectory.Create();
            }
            foreach (var file in _saveDirectory.EnumerateFiles())
            {
                if (AllowedExtensions.Contains(file.Extension))
                {
                    IFImage.Metadata metadata = new IFImage.Metadata(file);
                    _metadataStore.Add(file.FullName, metadata);
                }
            }
        }

        public IEnumerable<IFImage.Metadata> AllMetadata()
        {
            return _metadataStore.Values;
        }
    }
}