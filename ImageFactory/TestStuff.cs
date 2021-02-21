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

            /* float v = -3;

            for (int i = 0; i < 4; i++)
            {
                var ifs = _ifSpritePool.Spawn();
                ifs.Image = _imageManager.LoadedImages().ElementAt(i);
                ifs.Position = new UnityEngine.Vector3(v, 5f, 0f);
                v++;
            }*/
        }
    }
}
