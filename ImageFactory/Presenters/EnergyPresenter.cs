using System;
using System.Collections.Generic;
using System.Linq;
using ImageFactory.Components;
using ImageFactory.Managers;
using ImageFactory.Models;
using Zenject;

namespace ImageFactory.Presenters
{
	internal class EnergyPresenter : IInitializable, IDisposable
	{
		public const string ENERGY_ID = "Energy";
		public const string ENERGY_FULL_ID = "Enery Full";
		public const string ENERGY_RANGE_ID = "Energy Range";
		public const string ENERGY_DEPLETED_ID = "Energy Depleted";

		private readonly Config _config;
		private readonly ImageManager _imageManager;
		private readonly GameEnergyCounter _gameEnergyCounter;
		private readonly List<SaveImage> _savedImages = new List<SaveImage>();
		private readonly Dictionary<SaveImage, IFSprite> _activeSprites = new Dictionary<SaveImage, IFSprite>();

		public EnergyPresenter(Config config, ImageManager imageManager, GameEnergyCounter gameEnergyCounter)
		{
			_config = config;
			_imageManager = imageManager;
			_gameEnergyCounter = gameEnergyCounter;
		}

		public async void Initialize()
		{
			// Parse Energy Presentation Data
			var saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == ENERGY_ID);
			foreach (var save in saves)
			{
				IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
				if (metadata.HasValue)
				{
					var image = await _imageManager.LoadImage(metadata.Value);
					if (image != null)
					{
						string[] split = save.Presentation.Value.Split('|');
						if (split.Length != 2)
							continue;
						if (Enum.TryParse(split[0], out Mode mode) && float.TryParse(split[1], out float val))
						{
							float? minVal = null;
							if (mode == Mode.Above)
								minVal = val;
							float? maxVal = null;
							if (mode == Mode.Below)
								maxVal = val;

							// If for some reason we dont have a min or a max, move onto the next save (manual config editing gone wrong, most likely)
							if (!minVal.HasValue && !maxVal.HasValue && mode != Mode.Between)
								continue;

							var saveImage = new SaveImage(image, save, mode, minVal, maxVal);
							_savedImages.Add(saveImage);
						}
					}
				}
			}
			
			// Parse Energy Full Presentation Data
			saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == ENERGY_FULL_ID);
			foreach (var save in saves)
			{
				IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
				if (metadata.HasValue)
				{
					var image = await _imageManager.LoadImage(metadata.Value);
					if (image != null)
					{
						var saveImage = new SaveImage(image, save, Mode.Full);
						_savedImages.Add(saveImage);
					}
				}
			}

			// Parse Energy Range Presentation Data
			saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == ENERGY_RANGE_ID);
			foreach (var save in saves)
			{
				IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
				if (metadata.HasValue)
				{
					var image = await _imageManager.LoadImage(metadata.Value);
					if (image != null)
					{
						string[] split = save.Presentation.Value.Split('|');
						if (split.Length != 2)
							continue;
						if (float.TryParse(split[0], out float minVal) && float.TryParse(split[1], out float maxVal))
						{
							var saveImage = new SaveImage(image, save, Mode.Between, minVal, maxVal);
							_savedImages.Add(saveImage);
						}
					}
				}
			}
			
			// Parse Energy Depleted Presentation Data
			saves = _config.SaveData.Where(sd => sd.Enabled && sd.Presentation.PresentationID == ENERGY_DEPLETED_ID);
			foreach (var save in saves)
			{
				IFImage.Metadata? metadata = _imageManager.GetMetadata(save);
				if (metadata.HasValue)
				{
					var image = await _imageManager.LoadImage(metadata.Value);
					if (image != null)
					{
						var saveImage = new SaveImage(image, save, Mode.Depleted);
						_savedImages.Add(saveImage);
					}
				}
			}
			
			_gameEnergyCounter.gameEnergyDidChangeEvent += GameEnergyCounter_gameEnergyDidChangeEvent;
		}

		private void GameEnergyCounter_gameEnergyDidChangeEvent(float energy)
		{
			foreach (var image in _savedImages)
			{
				bool isValid = false;
				if (image.Mode == Mode.Full)
					isValid = energy >= 1f;
				else if (image.Mode == Mode.Depleted)
					isValid = energy <= 0f;
				else if (image.Mode == Mode.Between)
				{
					// If for some reason we dont have a min or a max, just don't do anything.
					if (!image.Min.HasValue || !image.Max.HasValue)
						continue;
					isValid = energy >= image.Min && image.Max >= energy;
				}
				else
				{
					if (image.Mode == Mode.Above && image.Min.HasValue)
						isValid = energy >= image.Min;
					else if (image.Mode == Mode.Below && image.Max.HasValue)
						isValid = image.Max >= energy;
				}

				if (isValid)
				{
					if (_activeSprites.ContainsKey(image))
						continue;
					var sprite = _imageManager.Spawn(image.SaveData);
					_activeSprites.Add(image, sprite);
					sprite.Image = image.Image;
				}
				else
				{
					if (_activeSprites.TryGetValue(image, out IFSprite sprite))
					{
						_activeSprites.Remove(image);
						_imageManager.Despawn(sprite);
					}
				}
			}
		}
		
		public void Dispose()
		{
			foreach (var sprite in _activeSprites)
				_imageManager.Despawn(sprite.Value, true);
			
			_gameEnergyCounter.gameEnergyDidChangeEvent -= GameEnergyCounter_gameEnergyDidChangeEvent;
		}

		private class SaveImage
		{
			public Mode Mode { get; }
			public float? Min { get; }
			public float? Max { get; }
			public IFImage Image { get; }
			public IFSaveData SaveData { get; }

			public SaveImage(IFImage image, IFSaveData saveData, Mode mode, float? min = null, float? max = null)
			{
				Min = min;
				Max = max;
				Mode = mode;
				Image = image;
				SaveData = saveData;
			}
		}

		private enum Mode
		{
			Above,
			Below,
			Between,
			Full,
			Depleted
		}
	}
}