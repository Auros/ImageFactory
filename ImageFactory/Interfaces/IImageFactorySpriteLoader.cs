using ImageFactory.Models;
using System.Threading.Tasks;

namespace ImageFactory.Interfaces
{
    public interface IImageFactorySpriteLoader
    {
        public Task<IFImage?> LoadAsync(IFImage.Metadata metadata);
    }
}