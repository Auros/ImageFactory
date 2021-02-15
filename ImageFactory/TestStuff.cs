using ImageFactory.Components;
using ImageFactory.Managers;
using SiraUtil.Tools;
using System.Linq;
using Zenject;

namespace ImageFactory
{
    internal class TestStuff : IInitializable
    {
        private readonly SiraLog _siraLog;
        private readonly ImageManager _imageManager;
        private readonly IFSprite.Pool _ifSpritePool;

        public TestStuff(SiraLog siraLog, ImageManager imageManager, IFSprite.Pool ifSpritePool)
        {
            _siraLog = siraLog;
            _imageManager = imageManager;
            _ifSpritePool = ifSpritePool;
        }

        public async void Initialize()
        {
            await SiraUtil.Utilities.AwaitSleep(3000);

            var ifs = _ifSpritePool.Spawn();
            ifs.Image = _imageManager.LoadedImages().First();
            ifs.Position = new UnityEngine.Vector3(0f, 5f, 0f);
        }
    }
}
