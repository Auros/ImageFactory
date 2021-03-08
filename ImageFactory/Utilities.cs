using BeatSaberMarkupLanguage.Animations;
using HMUI;
using ImageFactory.Models;
using IPA.Utilities;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ImageFactory
{
    internal static class Utilities
    {
        internal static readonly FieldAccessor<TableView, TableViewScroller>.Accessor Scroller = FieldAccessor<TableView, TableViewScroller>.GetAccessor("scroller");
        internal static readonly FieldAccessor<TableViewScroller, Button>.Accessor PageUpButton = FieldAccessor<TableViewScroller, Button>.GetAccessor("_pageUpButton");
        internal static readonly FieldAccessor<TableViewScroller, Button>.Accessor PageDownButton = FieldAccessor<TableViewScroller, Button>.GetAccessor("_pageDownButton");
        internal static readonly FieldAccessor<TableViewScroller, bool>.Accessor HideScrollButtons = FieldAccessor<TableViewScroller, bool>.GetAccessor("_hideScrollButtonsIfNotNeeded");

        public static Task<ProcessedAnimation> ProcessAnimation(AnimationType type, byte[] data)
        {
            return Task.Run(() =>
            {
                TaskCompletionSource<ProcessedAnimation> source = new TaskCompletionSource<ProcessedAnimation>();
                AnimationLoader.Process(type, data, (Texture2D tex, Rect[] uvs, float[] delays, int width, int height) =>
                {
                    source.SetResult(new ProcessedAnimation(tex, uvs, delays, width, height));
                });
                return source.Task;
            });
        }

        private static Material _roundEdge = null!;
        public static Material UINoGlowRoundEdge
        {
            get
            {
                if (_roundEdge == null)
                {
                    _roundEdge = Resources.FindObjectsOfTypeAll<Material>().First(m => m.name == "UINoGlowRoundEdge");
                }
                return _roundEdge;
            }
        }

        private static Shader _shader = null!;
        public static Shader ImageShader
        {
            get
            {
                if (_shader == null)
                {
                    _shader = Resources.FindObjectsOfTypeAll<Shader>().First(s => s.name == "Custom/Sprite");
                }
                return _shader;
            }
        }

        public static void SetButtons(this TableView tableView, Button upButton, Button downButton)
        {
            var scroller = Scroller(ref tableView);
            HideScrollButtons(ref scroller) = false;

            PageUpButton(ref scroller) = upButton;
            PageDownButton(ref scroller) = downButton;
        }
    }
}