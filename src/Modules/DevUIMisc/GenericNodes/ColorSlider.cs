using DevInterface;
using RegionKit.Extras.FutileExtras;

namespace RegionKit.Modules.DevUIMisc.GenericNodes
{
public partial class ColorPicker
	{
		/// <summary>
		/// A more direct visual representation of a slider with a color value.
		/// </summary>
		public class ColorSlider : RectangularDevUINode
		{
			private DevUILabel _elemLabel;
			private FTexture _sliderImage;
			private FRectangle _outline;
			private FSprite _sliderSelection;
			private float _sliderValue;
			private bool _dragged;

			public float Value { get => _sliderValue; }

			public ColorSlider(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, float width, string label, GenerateTexture genTex, float defaultValue) : base(owner, IDstring, parentNode, pos, new(width, 15f))
			{
				_sliderValue = defaultValue;

				_elemLabel = new DevUILabel(owner, label, this, new(), 35f, label);
				subNodes.Add(_elemLabel);

				Texture2D _displayTexture = new(Mathf.RoundToInt(width - 40f), Mathf.RoundToInt(size.y), TextureFormat.RGBA32, false);
				// Create the preview
				_sliderImage = new FTexture(UpdateTexture(_displayTexture, genTex), IDstring) { anchorX = 0f, anchorY = 0f };
				Futile.stage.AddChild(_sliderImage);
				fSprites.Add(_sliderImage);
				

				_sliderSelection = new FSprite("pixel")
				{
					shader = rainWorld.Shaders["Inverted"],
					scaleX = 4f,
					scaleY = 15f,
					anchorY = 0f,
				};
				Futile.stage.AddChild(_sliderSelection);
				fSprites.Add(_sliderSelection);

				_outline = new FRectangle(new(_sliderImage.width, _sliderImage.height));
				foreach (FSprite sprite in _outline.Fsprites)
				{
					Futile.stage.AddChild(sprite);
					fSprites.Add(sprite);
				}
			}

			private Texture2D UpdateTexture(Texture2D tex, GenerateTexture genTex, Color color = default)
			{
				Color[] colors = new Color[tex.width * tex.height];
				for (int x = 0; x < tex.width; x++)
				{
					for (int y = 0; y < tex.height; y++)
					{
						colors[x + tex.width * y] = genTex((float)x / (float)tex.width, color);
					}
				}
				tex.SetPixels(colors);
				tex.Apply(false);
				return tex;
			}

			/// <summary>
			/// Update the label and texture as needed.
			/// </summary>
			/// <param name="text"></param>
			/// <param name="genTex"></param>
			/// <param name="color"></param>
			public void ChangeSlider(string text, GenerateTexture genTex, Color color = default)
			{
				_elemLabel.Text = text;
				if (_sliderImage.element?.atlas?.texture is Texture2D tex)
				{
					_sliderImage.SetTexture(UpdateTexture(tex, genTex, color));
				}
			}

			public void Show(bool show)
			{
				foreach (var item in _elemLabel.fLabels)
				{
					item.isVisible = show;
				}
				foreach (var item in _elemLabel.fSprites)
				{
					item.isVisible = show;
				}
				_outline.Show(show);
				_sliderImage.isVisible = show;
				_sliderSelection.isVisible = show;
			}

			public override void Refresh()
			{
				_sliderImage.SetPosition(absPos + new Vector2(40f, 0f));
				_outline.SetPosition(_sliderImage.GetPosition());

				Vector2 imagePos = _sliderImage.GetPosition();
				_sliderSelection.SetPosition(new(Mathf.Lerp(imagePos.x + 2f, imagePos.x + _sliderImage.width - 2f, _sliderValue), imagePos.y));
				base.Refresh();
			}

			public virtual void UpdateValue(float value)
			{
				_sliderValue = value;
				Refresh();
			}

			public override void Update()
			{
				if (_sliderImage != null)
				{
					if (_sliderImage.MouseOver() && Input.GetMouseButtonDown(0))
					{
						_dragged = true;
					}
					else if (!Input.GetMouseButton(0))
					{
						_dragged = false;
					}

					if (_dragged)
					{
						Vector2 mousePos = Futile.mousePosition;
						Vector2 imagePos = _sliderImage.GetPosition();
						float relativeXPos = InverseLerpUnclamped(imagePos.x + 2f, imagePos.x + _sliderImage.width - 2f, mousePos.x);
						_sliderValue = Mathf.Clamp01(relativeXPos);
						this.SendSignal(DevUISignalType.ButtonClick, this, _sliderValue.ToString());
						Refresh();
					}
				}
				base.Update();
			}

			/// <summary>
			/// A function to generate a linear 2d rectangular texture based on a 0-1 float value. Also includes an optional <see cref="Color"/> parameter for updating textures.
			/// </summary>
			/// <param name="lerp"></param>
			/// <param name="color"></param>
			public delegate Color GenerateTexture(float lerp, Color color = default);

			public static Color GenerateAlpha(float lerp, Color color = default)
			{
				return new(1f, 1f, 1f, lerp);
			}

			// RGB
			public static Color GenerateRed(float lerp, Color color = default)
			{
				return new(Mathf.Lerp(0f, 1f, lerp), color.g, color.b);
			}
			public static Color GenerateGreen(float lerp, Color color = default)
			{
				return new(color.r, Mathf.Lerp(0f, 1f, lerp), color.b);
			}
			public static Color GenerateBlue(float lerp, Color color = default)
			{
				return new(color.r, color.g, Mathf.Lerp(0f, 1f, lerp));
			}

			// HSL
			public static Color GenerateHue(float lerp, Color color = default)
			{
				return new HSLColor(lerp, 1f, 0.5f).rgb;
			}
			public static Color GenerateSat(float lerp, Color color = default)
			{
				HSLColor hsl = color.HSL();
				return new HSLColor(hsl.hue, lerp, hsl.lightness).rgb;
			}
			public static Color GenerateLit(float lerp, Color color = default)
			{
				HSLColor hsl = color.HSL();
				return new HSLColor(hsl.hue, hsl.saturation, lerp).rgb;
			}
		}
	}
}
