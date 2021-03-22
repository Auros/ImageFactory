using ImageFactory.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using SiraUtil.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace ImageFactory.Presenters
{
    internal class ScenePresenter : IInitializable, IDisposable
    {
        private readonly Config _config;
        private readonly SiraLog _siraLog;
        private readonly ImageManager _imageManager;
        private readonly PlayerDataModel _playerDataModel;

        private readonly List<SaveSprite> _gameSprites;
        private readonly List<SaveSprite> _menuSprites;
        private readonly List<SaveSprite> _globalSprites;

        public const string GAME_ID = "In Song";
        public const string MENU_ID = "In Menu";
        public const string EVERYWHERE_ID = "Everywhere";

        public ScenePresenter(SiraLog siraLog, Config config, ImageManager imageManager, PlayerDataModel playerDataModel)
        {
            _config = config;
            _siraLog = siraLog;
            _imageManager = imageManager;
            _playerDataModel = playerDataModel;

            _gameSprites = new List<SaveSprite>();
            _menuSprites = new List<SaveSprite>();
            _globalSprites = new List<SaveSprite>();
        }

        public void Initialize()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            _imageManager.ImageUpdated += ImageManager_ImageUpdated;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
        {
            if (oldScene.name == "GameCore")
            {
                foreach (var gameSprite in _gameSprites)
                    _imageManager.Despawn(gameSprite.Sprite);
                _gameSprites.Clear();
            }
            else if (oldScene.name == "MenuViewControllers")
            {
                foreach (var menuSprite in _menuSprites)
                    menuSprite.Sprite.gameObject.SetActive(false);
            }
            else if (newScene.name == "MenuViewControllers")
            {
                foreach (var menuSprite in _menuSprites)
                    menuSprite.Sprite.gameObject.SetActive(true);
            }
        }

        private async void SceneManager_sceneLoaded(Scene scene, LoadSceneMode __)
        {
            if (scene.name == "MenuCore")
            {
                _globalSprites.AddRange(await LoadSprites(EVERYWHERE_ID));
                _menuSprites.AddRange(await LoadSprites(MENU_ID));
            }
            if (scene.name == "GameCore" && !(!_config.IgnoreTextAndHUDs && _playerDataModel.playerData.playerSpecificSettings.noTextsAndHuds))
            {
                _gameSprites.AddRange(await LoadSprites(GAME_ID));
            }
        }

        // This subscription handles updating any image in global or menu that have been
        // added, edited, or deleted in the UI.
        private void ImageManager_ImageUpdated(object sender, ImageUpdateArgs e)
        {
            if (e.Type == ImageUpdateArgs.Action.Added && e.SaveData.Enabled)
            {
                if (e.SaveData.Presentation.PresentationID == EVERYWHERE_ID)
                    SpawnThenAddSprite(e.SaveData, e.Image, _globalSprites);
                else if (e.SaveData.Presentation.PresentationID == MENU_ID)
                    SpawnThenAddSprite(e.SaveData, e.Image, _menuSprites);
            }
            else if (e.Type == ImageUpdateArgs.Action.Updated)
            {
                SaveSprite? save = _globalSprites.FirstOrDefault(ss => ss.SaveData == e.SaveData);
                if (save != null)
                {
                    if (e.SaveData.Presentation.PresentationID != EVERYWHERE_ID || !e.SaveData.Enabled)
                    {
                        _globalSprites.Remove(save);
                        if (!e.SaveData.Enabled || e.SaveData.Presentation.PresentationID != MENU_ID)
                        {
                            _imageManager.Despawn(save.Sprite);
                            return;
                        }
                        else if (e.SaveData.Enabled)
                        {
                            _globalSprites.Add(save);
                            UpdateSpriteData(save.Sprite, e.SaveData);
                        }
                    }
                    else
                    {
                        UpdateSpriteData(save.Sprite, e.SaveData);
                    }
                    return;
                }
                save = _menuSprites.FirstOrDefault(ss => ss.SaveData == e.SaveData);
                if (save != null)
                {
                    if (e.SaveData.Presentation.PresentationID != MENU_ID || !e.SaveData.Enabled)
                    {
                        _menuSprites.Remove(save);
                        if (!e.SaveData.Enabled || e.SaveData.Presentation.PresentationID != EVERYWHERE_ID)
                        {
                            _imageManager.Despawn(save.Sprite);
                            return;
                        }
                        else if (e.SaveData.Enabled)
                        {
                            _globalSprites.Add(save);
                            UpdateSpriteData(save.Sprite, e.SaveData);
                        }
                    }
                    else
                    {
                        UpdateSpriteData(save.Sprite, e.SaveData);
                    }
                    return;
                }
                if (e.SaveData.Enabled)
                {
                    if (e.SaveData.Presentation.PresentationID == EVERYWHERE_ID)
                        SpawnThenAddSprite(e.SaveData, e.Image, _globalSprites);
                    else if (e.SaveData.Presentation.PresentationID == MENU_ID)
                        SpawnThenAddSprite(e.SaveData, e.Image, _menuSprites);
                }
            }
            else if (e.Type == ImageUpdateArgs.Action.Removed)
            {
                SaveSprite? save = _globalSprites.FirstOrDefault(ss => ss.SaveData == e.SaveData);
                if (save != null)
                {
                    _globalSprites.Remove(save);
                    _imageManager.Despawn(save.Sprite);
                    return;
                }
                save = _menuSprites.FirstOrDefault(ss => ss.SaveData == e.SaveData);
                if (save != null)
                {
                    _menuSprites.Remove(save);
                    _imageManager.Despawn(save.Sprite);
                }
            }
        }

        private async Task<List<SaveSprite>> LoadSprites(string id)
        {
            List<SaveSprite> sprites = new List<SaveSprite>();
            var saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == id);
            foreach (var save in saves)
            {
                IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
                if (metadata.HasValue)
                {
                    var image = await _imageManager.LoadImage(metadata.Value);
                    if (image != null)
                        SpawnThenAddSprite(save, image, sprites);
                }
            }
            return sprites;
        }

        private void SpawnThenAddSprite(IFSaveData save, IFImage image, List<SaveSprite> collection)
        {
            var sprite = _imageManager.Spawn(save);
            sprite.Image = image;
            collection.Add(new SaveSprite(sprite, save));
        }

        private void UpdateSpriteData(IFSprite sprite, IFSaveData newData)
        {
            sprite.Position = newData.Position;
            sprite.Rotation = newData.Rotation;
            sprite.Size = newData.Size;
        }

        public void Dispose()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            _imageManager.ImageUpdated -= ImageManager_ImageUpdated;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private class SaveSprite
        {
            public IFSprite Sprite { get; }
            public IFSaveData SaveData { get; }

            public SaveSprite(IFSprite sprite, IFSaveData saveData)
            {
                Sprite = sprite;
                SaveData = saveData;
            }
        }
    }
}