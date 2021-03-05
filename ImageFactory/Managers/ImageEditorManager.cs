using ImageFactory.Components;
using ImageFactory.Models;
using System;
using UnityEngine;

namespace ImageFactory.Managers
{
    internal class ImageEditorManager
    {
        private Action? _saveAction;
        private IFSaveData? _lastClone;
        private IFSprite _activeSprite = null!;
        private readonly IFSprite.Pool _spritePool;

        public ImageEditorManager(IFSprite.Pool spritePool)
        {
            _spritePool = spritePool;
        }

        public Transform Present(IFImage image, IFSaveData saveData, Action<IFSaveData> saved)
        {
            if (_activeSprite != null)
            {
                _spritePool.Despawn(_activeSprite);
            }
            _lastClone = new IFSaveData
            {
                Name = saveData.Name,
                Size = saveData.Size,
                Enabled = saveData.Enabled,
                Position = saveData.Position,
                Rotation = saveData.Rotation,
                Presentation = saveData.Presentation,
                LocalFilePath = saveData.LocalFilePath
            };
            _saveAction = delegate () { saved?.Invoke(_lastClone!); };
            _activeSprite = _spritePool.Spawn();
            _activeSprite.Image = image;
            _activeSprite.Position = _lastClone.Position;
            _activeSprite.Rotation = _lastClone.Rotation;
            _activeSprite.Size = _lastClone.Size;
            return _activeSprite.transform;
        }

        public void Dismiss(bool save = true)
        {
            if (_lastClone != null)
            {
                Position = Position;
                Rotation = Rotation;
            }
            if (save)
            {
                _saveAction?.Invoke();
            }
            if (_activeSprite != null)
            {
                _activeSprite.transform.SetParent(null);
                _spritePool.Despawn(_activeSprite);
            }
            _activeSprite = null!;
        }

        #region Properties

        public bool Enabled
        {
            get => _lastClone?.Enabled ?? default;
            set { if (_lastClone != null) _lastClone.Enabled = value; }
        }

        public string Name
        {
            get => _lastClone?.Name ?? "NOT SET";
            set { if (_lastClone != null) _lastClone.Name = value; }
        }

        public Vector2 Size
        {
            get => (_activeSprite != null) ? _activeSprite.Size : default;
            set
            {
                if (_activeSprite != null && _lastClone != null)
                {
                    _activeSprite.Size = value;
                    _lastClone.Size = value;
                }
            }
        }

        public Vector3 Position
        {
            get => (_activeSprite != null) ? _activeSprite.Position : default;
            set
            {
                if (_activeSprite != null && _lastClone != null)
                {
                    _activeSprite.Position = value;
                    _lastClone.Position = value;
                }
            }
        }

        public Quaternion Rotation
        {
            get => (_activeSprite != null) ? _activeSprite.Rotation : default;
            set
            {
                if (_activeSprite != null && _lastClone != null)
                {
                    _activeSprite.Rotation = value;
                    _lastClone.Rotation = value;
                }
            }
        }

        #endregion
    }
}