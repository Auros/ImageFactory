using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace ImageFactory.UI
{
    [ViewDefinition("ImageFactory.Views.info-view.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\info-view.bsml")]
    internal class IFInfoView : BSMLAutomaticViewController
    {

    }
}