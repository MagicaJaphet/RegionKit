using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevInterface;
using Unity.Mathematics;
using UnityEngine;

namespace RegionKit.Modules.DevUIMisc.GenericNodes
{
	/// <summary>
	/// A fleshed-out color picker Dev Tool node, based on those used by modern art programs.
	/// </summary>
	public partial class ColorPicker : RectangularDevUINode, IDevUISignals
	{
		// Shader values
		public static readonly float __HueCircleInner = 0.4f;
		public static readonly float __SVSquareSize = 0.25f;

		// Class values
		public static Vector2 ElementSize(bool alpha) => alpha ? new(155f, 215f) : new(155f, 195f);
		private static readonly float __ColorWheelScale = ElementSize(false).x - 10f;
		private Button _colorWheelMode;
		private StringControl _hexinput;
		private FSprite _colorPicker;
		private const float Recip2Pi = 0.5f / Mathf.PI;
		private const float TwoPi = Mathf.PI * 2f;

		private enum ColorMode
		{
			Wheel,
			RGB,
			HSL
		}
		private ColorMode _colorMode = ColorMode.Wheel;
		private bool _colorModeNeedsUpdate;
		private FSprite _hueSelectorLine;
		private FSprite _valueSelectorCircle;
		private int _drag = -1;
		internal FSprite colorPreview;

		private static Color _defaultColor = Color.red;
		private bool _alpha;

		public Color ColorValue 
		{
			get => _colorValue; 
			set 
			{ 
				_colorValue = value; 
				UpdateColor(); 
			} 
		}
		private Color _colorValue = _defaultColor;
		private ColorSlider? _alphaSlider;
		private Button _hslMode;
		private Button _rgbMode;
		private ColorSlider _sliderOne;
		private ColorSlider _sliderTwo;
		private ColorSlider _sliderThree;

		public ColorPicker(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, bool alpha) : base(owner, IDstring, parentNode, pos, ElementSize(alpha))
		{
			_alpha = alpha;
			Vector2 elemSize = ElementSize(alpha);

			// Color Mode buttoons
			_colorWheelMode = new Button(owner, "color_wheel_selection", this, new(0f, elemSize.y - 16f), elemSize.x / 3f, "Wheel");
			subNodes.Add(_colorWheelMode);
			_rgbMode = new Button(owner, "rgb_selection", this, new(elemSize.x / 3f, elemSize.y - 16f), elemSize.x / 3f, "RGB");
			subNodes.Add(_rgbMode);
			_hslMode = new Button(owner, "hsl_selection", this, new(elemSize.x - (elemSize.x / 3f), elemSize.y - 16f), elemSize.x / 3f, "HSL");
			subNodes.Add(_hslMode);

			// Hex input
			_hexinput = new StringControl(owner, "hex_input", this, new Vector2(72f, 0f), 82f, colorToHex(_defaultColor), alpha ? StringControl.TextIsColorWithAlpha : StringControl.TextIsColor);
			subNodes.Add(_hexinput);

			// Color Picker sprites

			_colorPicker = new("Futile_White")
			{
				width = __ColorWheelScale,
				height = __ColorWheelScale,
				shader = rainWorld.Shaders["ColorPicker"],
				color = ColorValue
			};
			Futile.stage.AddChild(_colorPicker);
			fSprites.Add(_colorPicker);

			_hueSelectorLine = new FSprite("pixel")
			{
				scaleX = 4f,
				scaleY = (__HueCircleInner * (__ColorWheelScale / 2f)) / 2f,
				_anchorY = 0,
				shader = rainWorld.Shaders["Inverted"]
			};
			Futile.stage.AddChild(_hueSelectorLine);
			fSprites.Add(_hueSelectorLine);

			_valueSelectorCircle = new FSprite("mouseEyeB5")
			{
				width = _hueSelectorLine.scaleY,
				height = _hueSelectorLine.scaleY,
				rotation = 45f,
				shader = rainWorld.Shaders["Inverted"]
			};
			Futile.stage.AddChild(_valueSelectorCircle);
			fSprites.Add(_valueSelectorCircle);

			FSprite previewBacking = new("pixel")
			{
				scaleX = 40f,
				scaleY = 20f,
				anchorX = 0,
				anchorY = 0,
				color = Color.grey
			};
			Futile.stage.AddChild(previewBacking);
			fSprites.Add(previewBacking);
			colorPreview = new FSprite("pixel")
			{
				scaleX = 40f,
				scaleY = 20f,
				anchorX = 0,
				anchorY = 0,
			};
			Futile.stage.AddChild(colorPreview);
			fSprites.Add(colorPreview);

			// Color sliders
			float y = elemSize.y - 16f - 15f - 10f;
			_sliderOne = new ColorSlider(owner, "red_or_hue_slider", this, new(0f, y), elemSize.x, "Red", ColorSlider.GenerateRed, _defaultColor.r);
			subNodes.Add(_sliderOne);

			y -= 50f;
			_sliderTwo = new ColorSlider(owner, "green_or_sat_slider", this, new(0f, y), elemSize.x, "Green", ColorSlider.GenerateGreen, _defaultColor.g);
			subNodes.Add(_sliderTwo);

			y -= 50f;
			_sliderThree = new ColorSlider(owner, "blue_or_light_slider", this, new(0f, y), elemSize.x, "Blue", ColorSlider.GenerateBlue, _defaultColor.b);
			subNodes.Add(_sliderThree);

			if (alpha)
			{
				_alphaSlider = new ColorSlider(owner, "alpha_slider", this, new(0f, 25f), elemSize.x, "Alpha", ColorSlider.GenerateAlpha, _defaultColor.a);
				subNodes.Add(_alphaSlider);
			}
			_colorModeNeedsUpdate = true;
			UpdateColor();
		}

		private Color HSLToColor(HSLColor color, float alpha)
		{
			Color c = color.rgb;
			return new(c.r, c.g, c.b, alpha);
		}
		private Color HSVToColor(HSVColor color, float alpha)
		{
			Color c = color.rgb;
			return new(c.r, c.g, c.b, alpha);
		}

		public void Signal(DevUISignalType type, DevUINode sender, string message)
		{
			if (sender == _hexinput)
			{
				ColorValue = _alpha && message.Length == 8 ? hexToColorWithAlpha(_hexinput.actualValue) : hexToColor(_hexinput.actualValue);
			}
			if (_alpha && sender == _alphaSlider)
			{
				ColorValue = new(ColorValue.r, ColorValue.g, ColorValue.b, float.Parse(message));
			}

			float alpha = _alphaSlider?.Value ?? 1f;
			HSLColor hsl = ColorValue.HSL();
			// Red / Hue
			if (sender == _sliderOne)
			{
				if (_colorMode == ColorMode.RGB)
				{
					ColorValue = new(float.Parse(message), ColorValue.g, ColorValue.b, alpha);
				}
				else
				{
					ColorValue = HSLToColor(new HSLColor(float.Parse(message), hsl.saturation, hsl.lightness), alpha);
				}
			}
			// Green / Saturation
			if (sender == _sliderTwo)
			{
				if (_colorMode == ColorMode.RGB)
				{
					ColorValue = new(ColorValue.r, float.Parse(message), ColorValue.b, alpha);
				}
				else
				{
					ColorValue = HSLToColor(new HSLColor(hsl.hue, Mathf.Clamp(float.Parse(message), 0.001f, 1f), hsl.lightness), alpha);
				}
			}
			// Blue / Lightness
			if (sender == _sliderThree)
			{
				if (_colorMode == ColorMode.RGB)
				{
					ColorValue = new(ColorValue.r, ColorValue.g, float.Parse(message), alpha);
				}
				else
				{
					ColorValue = HSLToColor(new HSLColor(hsl.hue, hsl.saturation, Mathf.Clamp(float.Parse(message), 0.001f, 0.999f)), alpha);
				}
			}

			if (sender == _colorWheelMode)
			{
				_colorMode = ColorMode.Wheel;
				_colorModeNeedsUpdate = true;
			}
			if (sender == _rgbMode)
			{
				_colorMode = ColorMode.RGB;
				_colorModeNeedsUpdate = true;
			}
			if (sender == _hslMode)
			{
				_colorMode = ColorMode.HSL;
				_colorModeNeedsUpdate = true;
			}
		}

		public override void Refresh()
		{
			MoveSprite(0, absPos + new Vector2((ElementSize(_alpha) / 2f).x, (_alpha ? 45f : 25f) + (__ColorWheelScale / 2f)));
			MoveSprite(3, absPos);
			colorPreview.SetPosition(absPos);

			if (_colorPicker != null && _hueSelectorLine != null && _valueSelectorCircle != null)
			{
				HSVColor hsv = ColorValue.HSV();
				_hueSelectorLine.SetPosition(_colorPicker.GetPosition() + (new Vector2(Mathf.Cos(hsv.hue * TwoPi), Mathf.Sin(hsv.hue * TwoPi)) * ((__ColorWheelScale / 2f) - (_hueSelectorLine.scaleY / 2f))));
				_hueSelectorLine.rotation = AimFromOneVectorToAnother(_colorPicker.GetPosition(), _hueSelectorLine.GetPosition());

				Vector2 square = new Vector2(__SVSquareSize, __SVSquareSize) * __ColorWheelScale;
				_valueSelectorCircle.SetPosition(_colorPicker.GetPosition() + new Vector2(Mathf.Lerp(-square.x, square.y, hsv.saturation), Mathf.Lerp(-square.y, square.y, hsv.value)));
				UpdatePreview();
			}
			base.Refresh();
		}

		public virtual void UpdatePreview()
		{
			colorPreview.color = ColorValue;
			colorPreview.alpha = ColorValue.a;
		}

		public override void Update()
		{
			if (_colorModeNeedsUpdate)
			{
				_colorModeNeedsUpdate = false;

				_colorPicker.isVisible = _colorMode == ColorMode.Wheel;
				_hueSelectorLine.isVisible = _colorPicker.isVisible;
				_valueSelectorCircle.isVisible = _colorPicker.isVisible;

				_sliderOne.Show(_colorMode != ColorMode.Wheel);
				_sliderTwo.Show(_colorMode != ColorMode.Wheel);
				_sliderThree.Show(_colorMode != ColorMode.Wheel);
				UpdateColor();
			}

			if (_colorMode == ColorMode.Wheel)
			{
				if (_colorPicker != null)
				{
					Vector2 sprPos = _colorPicker.GetBottomLeftPos();
					Vector2 mousePos = Futile.mousePosition;
					Vector2 relativePosition = new Vector2(InverseLerpUnclamped(sprPos.x, sprPos.x + _colorPicker.width, mousePos.x), InverseLerpUnclamped(sprPos.y, sprPos.y + _colorPicker.height, mousePos.y)) - Vector2.one * 0.5f;
					float r = relativePosition.magnitude;

					HSVColor hsv = ColorValue.HSV();
					bool updateColor = false;
					if (_colorPicker.MouseOver())
					{
						if (r < 0.5f && r > __HueCircleInner && Input.GetMouseButtonDown(0))
							_drag = 0;
						else if (relativePosition.x >= -__SVSquareSize && relativePosition.x <= __SVSquareSize && relativePosition.y >= -__SVSquareSize && relativePosition.y <= __SVSquareSize && Input.GetMouseButtonDown(0))
							_drag = 1;
					}

					switch (_drag)
					{
						case 0:
							hsv.hue = (Mathf.Atan2(relativePosition.y, relativePosition.x) * Recip2Pi + 1) % 1;
							updateColor = true;
							break;

						case 1:
							float size = __SVSquareSize;
							hsv.saturation = Mathf.Clamp(Mathf.InverseLerp(-size, size, relativePosition.x), 0.001f, 1f);
							hsv.value = Mathf.Clamp(Mathf.InverseLerp(-size, size, relativePosition.y), 0.001f, 0.99f);
							updateColor = true;
							break;
					}

					if (updateColor)
					{
						ColorValue = HSVToColor(hsv, ColorValue.a);
					}
					if (!Input.GetMouseButton(0))
					{
						_drag = -1;
					}
				}
			}

			base.Update();
		}

		internal virtual void UpdateColor()
		{
			_colorPicker.color = ColorValue;
			_hexinput.actualValue = _alpha && ColorValue.a < 1f ? colorToHexWithAlpha(ColorValue) : colorToHex(ColorValue);
			_hexinput.Text = _hexinput.actualValue;
			if (_colorMode == ColorMode.RGB)
			{
				_sliderOne.ChangeSlider("Red", ColorSlider.GenerateRed, ColorValue);
				_sliderTwo.ChangeSlider("Green", ColorSlider.GenerateGreen, ColorValue);
				_sliderThree.ChangeSlider("Blue", ColorSlider.GenerateBlue, ColorValue);
			}
			else if (_colorMode	== ColorMode.HSL)
			{
				_sliderOne.ChangeSlider("Hue", ColorSlider.GenerateHue);
				_sliderTwo.ChangeSlider("Sat", ColorSlider.GenerateSat, ColorValue);
				_sliderThree.ChangeSlider("Lit", ColorSlider.GenerateLit, ColorValue);
			}
			if (_alpha)
				_alphaSlider?.UpdateValue(ColorValue.a);
			this.SendSignal(DevUISignalType.ButtonClick, this, ColorValue.ToString());
			Refresh();
		}
	}
}
