using ImageFactory.Models;
using UnityEngine;
using Zenject;

namespace ImageFactory.Components
{
    internal class IFSprite : MonoBehaviour
    {
        private IFImage? _image;
        [SerializeField] private Shader _shader = null!;
        [SerializeField] private Material _material = null!;
        [SerializeField] private SpriteRenderer _spriteRenderer = null!;
        private RendererAnimationStateUpdater? _animator = null!;

        public Vector2 Size
        {
            get => new Vector2(transform.localScale.x, transform.localScale.y);
            set => transform.localScale = new Vector3(value.x, value.y, 1f);
        }

        public Vector3 Position
        {
            get => transform.position + new Vector3(_spriteRenderer.bounds.extents.x, _spriteRenderer.bounds.extents.y, 0);
            set => transform.position = value - new Vector3(_spriteRenderer.bounds.extents.x, _spriteRenderer.bounds.extents.y, 0);
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public IFImage? Image
        {
            get => _image;
            set
            {
                var oldPos = Position;
                _image = value;
                if (_image is null)
                {
                    _spriteRenderer.sprite = null!;
                    if (_animator != null)
                    {
                        _animator.enabled = false;
                    }
                }
                else
                { 
                    if (_image.metadata.animationType is null)
                    {
                        if (_animator != null)
                        {
                            _animator.enabled = false;
                        }
                        _spriteRenderer.sprite = _image.sprite;
                    }
                    else
                    {
                        _spriteRenderer.sprite = _image.animationData!.sprites[0];
                        if (_animator == null)
                        {
                            _animator = gameObject.AddComponent<RendererAnimationStateUpdater>();
                            _animator.Renderer = _spriteRenderer;
                        }
                        _animator.enabled = true;
                        _animator.ControllerData = _image.animationData;
                    }
                }
                Position = oldPos;
            }
        }

        internal void Setup(SpriteRenderer renderer, Material material, Shader shader)
        {
            _shader = shader;
            _material = material;
            _spriteRenderer = renderer;
        }

        protected void Start()
        {
            _spriteRenderer.material = _material;
            _spriteRenderer.material.shader = _shader;
        }

        public class Pool : MonoMemoryPool<IFSprite> { /*Initialize Pool Type*/ }
    }
}