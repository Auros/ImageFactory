using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using ImageFactory.Managers;
using ImageFactory.Models;
using IPA.Utilities;
using System;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Zenject;

namespace ImageFactory.UI
{
    [ViewDefinition("ImageFactory.Views.edit-image-view.bsml")]
    [HotReload(RelativePathToLayout = @"..\Views\edit-image-view.bsml")]
    internal class IFEditImageView : BSMLAutomaticViewController
    {
        public event Action? Cancelled;
        public event Action? Saved;

        private DiContainer _container = null!;
        private ViewController _dummyView = null!;
        private FloatingScreen _floatingScreen = null!; 
        private InputFieldView _editorFieldView = null!;
        private InputFieldView _templateFieldView = null!;
        private PresentationStore _presentationStore = null!;
        private ImageEditorManager _imageEditorManager = null!;

        [UIAction("cancel-clicked")]
        protected void CancelClicked() => Cancelled?.Invoke();

        [UIAction("save-clicked")]
        protected void SaveClicked() => Saved?.Invoke();

        [UIComponent("input-root")]
        protected readonly RectTransform _inputRoot = null!;

        [UIValue("scale-x")]
        protected float XScale
        {
            get => _imageEditorManager.Size.x;
            set { _imageEditorManager.Size = new Vector2(value, _imageEditorManager.Size.y); NotifyPropertyChanged(); }
        }

        [UIValue("scale-y")]
        protected float YScale
        {
            get => _imageEditorManager.Size.y;
            set { _imageEditorManager.Size = new Vector2(_imageEditorManager.Size.x, value); NotifyPropertyChanged(); }
        }

        [Inject]
        public void Construct(DiContainer container, PresentationStore presentationStore, ImageEditorManager imageEditorManager, PhysicsRaycasterWithCache cacheRaycaster, LevelSearchViewController levelSearchViewController)
        {
            _container = container;
            _presentationStore = presentationStore;
            _imageEditorManager = imageEditorManager;
            _templateFieldView = levelSearchViewController.GetField<InputFieldView, LevelSearchViewController>("_searchTextInputFieldView");
            _floatingScreen = FloatingScreen.CreateFloatingScreen(new Vector2(10, 10f), true, Vector3.zero, Quaternion.identity, 0f, false);
            _floatingScreen.GetComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", cacheRaycaster);
            _dummyView = BeatSaberUI.CreateViewController<ViewController>();
            _floatingScreen.transform.localScale = Vector3.one;
            _dummyView.name = "IF Editor DummyViewController";
            _floatingScreen.name = "IF Editor Cube";
        }

        protected void Start()
        {
            _floatingScreen.gameObject.SetActive(false);
        }

        public void EnableEditing(IFImage image)
        {
            // Uses our dummy view controller to use the BSML handle because I am lazy
            // Then, resize the handle accordingly, setup our editor sprite instance,
            // set the position of our handle TO the image and THEN make the sprite a
            // child of the handle screen.
            var saveData = new IFSaveData { Position = new Vector3(0f, 2f, 2f), Name = image.metadata.file.Name };
            _floatingScreen.gameObject.SetActive(true);
            _floatingScreen.SetRootViewController(_dummyView, AnimationType.None);
            _floatingScreen.handle.transform.localScale = Vector3.one / 5f;
            _floatingScreen.handle.gameObject.transform.localPosition = Vector3.zero;
            Transform tForm = _imageEditorManager.Present(image, saveData, SavedData);
            _floatingScreen.ScreenPosition = _imageEditorManager.Position;
            _floatingScreen.ScreenRotation = _imageEditorManager.Rotation;
            _floatingScreen.handle.gameObject.transform.localPosition = Vector3.zero;
            _floatingScreen.handle.gameObject.transform.position = saveData.Position;
            tForm.transform.SetParent(_floatingScreen.transform, true);
            _editorFieldView.SetText(_imageEditorManager.Name);
            XScale = XScale;
            YScale = YScale;

        }

        private void SavedData(IFSaveData saveData)
        {

        }

        private void NameFieldUpdated(InputFieldView field)
        {
            _imageEditorManager.Name = field.text;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            _editorFieldView.onValueChanged.AddListener(NameFieldUpdated);
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _imageEditorManager.SaveAndDismiss();
            _floatingScreen.SetRootViewController(null, AnimationType.None);
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
            _editorFieldView.onValueChanged.RemoveListener(NameFieldUpdated);
            _floatingScreen.gameObject.SetActive(false);
        }

        [UIAction("#post-parse")]
        protected void Parsed()
        {
            _inputRoot.GetComponent<ContentSizeFitter>().enabled = false;
            _editorFieldView = _container.InstantiatePrefabForComponent<InputFieldView>(_templateFieldView.gameObject, _inputRoot.transform);
            _editorFieldView.SetField("_keyboardPositionOffset", Vector3.zero);
            _editorFieldView.SetField("_textLengthLimit", 48);
            _editorFieldView.SetText(_imageEditorManager.Name);
        }
    }
}