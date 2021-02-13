using ImageFactory.Models;
using System.Threading.Tasks;

namespace ImageFactory.Interfaces
{
    public interface IImageFactorySpriteLoader
    {
        /// <summary>
        /// Loads an ImageFactory image (state object) based on metadata.
        /// </summary>
        /// <param name="metadata">The image metadata.</param>
        /// <param name="stateUpdater">Override the state updater. By default all implementations should have their own state updaters.</param>
        /// <returns>The image or null if deserialization and loading fails.</returns>
        public Task<IFImage?> LoadAsync(IFImage.Metadata metadata, IAnimationStateUpdater? stateUpdater = null);
    }
}