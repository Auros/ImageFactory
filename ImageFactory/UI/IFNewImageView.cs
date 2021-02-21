using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace ImageFactory.UI
{
    [HotReload(RelativePathToLayout = @"..\Views\new-image-view.bsml")]
    [ViewDefinition("ImageFactory.Views.new-image-view.bsml")]
    internal class IFNewImageView : BSMLAutomaticViewController
    {
    }
}