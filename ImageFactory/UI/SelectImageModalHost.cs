using BeatSaberMarkupLanguage.Animations;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using ImageFactory.Models;
using UnityEngine;
using UnityEngine.UI;

namespace ImageFactory.UI
{
    internal class SelectImageModalHost
    {
        [UIParams]
        protected readonly BSMLParserParams parserParams = null!;

        [UIComponent("preview")]
        protected readonly ImageView _preview = null!;

        [UIComponent("anim-text")]
        protected readonly CurvedTextMeshPro _animText = null!;

        [UIComponent("width-text")]
        protected readonly CurvedTextMeshPro _widthText = null!;

        [UIComponent("height-text")]
        protected readonly CurvedTextMeshPro _heightText = null!;

        [UIComponent("file-size-text")]
        protected readonly CurvedTextMeshPro _fileSizeText = null!;

        [UIComponent("load-time-text")]
        protected readonly CurvedTextMeshPro _loadTimeText = null!;

        private Material _originalMaterial = null!;
        private AnimationStateUpdater _animationState = null!;
        private AnimationControllerData? _lastControllerData;

        [UIAction("#post-parse")]
        protected void Parsed()
        {
            _originalMaterial = _preview.material;
            _preview.material = Utilities.UINoGlowRoundEdge;
            _animationState = _preview.gameObject.AddComponent<AnimationStateUpdater>();
        }

        public void Present(IFImage image)
        {
            if (_lastControllerData != null && _lastControllerData.activeImages.Contains(_preview))
                _lastControllerData.activeImages.Remove(_preview);
            if (image.animationData != null)
            {
                _preview.material = _originalMaterial;
                _lastControllerData = image.animationData;
                _lastControllerData.activeImages.Add(_preview);
                _animationState.image = _preview;
            }
            else
            {
                _preview.material = Utilities.UINoGlowRoundEdge;
                _preview.sprite = image.sprite;
                _animationState.image = null;
            }
            _preview.sprite = image.sprite;
            var fileSize = FileSizeExtension(image.metadata.size);
            _animText.text = $"Animated: {(image.animationData != null ? "Yes" : "No" )}";
            _widthText.text = $"Width: {image.width}px";
            _heightText.text = $"Height: {image.height}px";
            _fileSizeText.text = $"File Size: {Mathf.RoundToInt(image.metadata.size / (float)fileSize.Item1)} {fileSize.Item2}";
            _loadTimeText.text = $"Load Time: {image.loadTime.Milliseconds} ms";
            parserParams.EmitEvent("show-modal");
        }

        public void Dismiss()
        {
            parserParams.EmitEvent("hide-modal");
        }

        private (int, string) FileSizeExtension(long size)
        {
            if (size > 1000000)
                return (1000000, "MB");
            if (size > 1000)
                return (1000, "KB");
            else
                return (1, "bytes");
        }
    }
}