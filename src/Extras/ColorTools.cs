using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionKit.Extras
{
	/// <summary>
	/// Tools to extend Unity and RWCustom's Color helpers.
	/// </summary>
	internal static class ColorTools
	{
		/// <summary>
		/// Converts the <see cref="Vector3"/> created by <see cref="RGB2HSL(Color)"/> into a proper <see cref="HSLColor"/>.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static HSLColor HSL(this Color color)
		{
			Vector3 v = RGB2HSL(color);
			return new HSLColor(v.x, v.y, v.z);
		}

		public struct HSVColor
		{
			public float hue;

			public float saturation;

			public float value;

			public Color rgb => HSVtoRGB(hue, saturation, value);

			public HSVColor(float hue, float saturation, float value)
			{
				if (hue < 0f)
				{
					hue += Mathf.Floor(hue) + 3f;
				}

				this.hue = hue - Mathf.Floor(hue);
				this.saturation = Mathf.Clamp(saturation, 0f, 1f);
				this.value = Mathf.Clamp(value, 0f, 1f);
			}

			public static HSVColor Lerp(HSVColor from, HSVColor to, float lrp)
			{
				return new HSVColor((Mathf.LerpAngle(from.hue * 360f, to.hue * 360f, lrp) + 0f) / 360f, Mathf.Lerp(from.saturation, to.saturation, lrp), Mathf.Lerp(from.value, to.value, lrp));
			}
		}

		/// <summary>
		/// Converts <see cref="Color"/> to a HSV value, returned in a <see cref="Vector3"/>.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static HSVColor HSV(this Color c)
		{
			float r = c.r;
			float g = c.g;
			float b = c.b;

			float max = Mathf.Max(r, g, b);
			float min = Mathf.Min(r, g, b);
			float delta = max - min;

			float v = max;

			float s = 0;
			if (max == 0) s = 0;
			else s = delta / max;

			float h = 0;
			if (delta == 0) h = 0;
			else if (max == r) h = 60 * (((g - b) / delta) % 6);
			else if (max == g) h = 60 * (((b - r) / delta) + 2);
			else if (max == b) h = 60 * (((r - g) / delta) + 4);

			return new(h / 360f, s, v);
		}

		public static Color HSVtoRGB(float h, float s, float v)
		{
			// hue is calculated in angles, from 0 to 360
			h *= 360f;

			float C = v * s;
			float X = C * (1 - Mathf.Abs((h / 60f) % 2 - 1));
			float m = v - C;

			float r = 0;
			float g = 0;
			float b = 0;
			if (0 <= h && h < 60) { r = C; g = X; b = 0; }
			else if (60 <= h && h < 120) { r = X; g = C; b = 0; }
			else if (120 <= h && h < 180) { r = 0; g = C; b = X; }
			else if (180 <= h && h < 240) { r = 0; g = X; b = C; }
			else if (240 <= h && h < 300) { r = X; g = 0; b = C; }
			else if (300 <= h && h < 360) { r = C; g = 0; b = X; }

			return new(r + m, g + m, b + m);
		}

		public static Color hexToColorWithAlpha(string hex)
		{
			return new Color((float)Convert.ToInt32(hex.Substring(0, 2), 16) / 255f, (float)Convert.ToInt32(hex.Substring(2, 2), 16) / 255f, (float)Convert.ToInt32(hex.Substring(4, 2), 16) / 255f, hex.Length == 8 ? (float)Convert.ToInt32(hex.Substring(6, 2), 16) / 255f : 1f);
		}
		public static string colorToHexWithAlpha(Color col)
		{
			return Mathf.RoundToInt(col.r * 255f).ToString("X2") + Mathf.RoundToInt(col.g * 255f).ToString("X2") + Mathf.RoundToInt(col.b * 255f).ToString("X2") + Mathf.RoundToInt(col.a * 255f).ToString("X2");
		}

		/// <summary>
		/// Struct to perform <see cref="Color"/> operations that don't exist in the <see cref="Color"/> struct.
		/// </summary>
		internal struct ColorOperator
		{
			internal Color Color
			{
				get => _clr;
				set => _clr = value;
			}
			private Color _clr;

			internal ColorOperator Inverted { get => 1f - new ColorOperator(Color); }

			internal ColorOperator(float r, float g, float b) => _clr = new Color(r, g, b);
			internal ColorOperator(Color color) => _clr = color;

			public static ColorOperator operator +(float a, ColorOperator b)
			{
				return new(a + b._clr.r, a + b._clr.g, a + b._clr.b);
			}
			public static ColorOperator operator +(ColorOperator b, float a)
			{
				return new(a + b._clr.r, a + b._clr.g, a + b._clr.b);
			}

			public static bool operator <(float a, ColorOperator b)
			{
				return a < b._clr.r && a < b._clr.g && a < b._clr.b;
			}
			public static bool operator >(float a, ColorOperator b)
			{
				return a > b._clr.r && a > b._clr.g && a > b._clr.b;
			}
			public static bool operator <(ColorOperator b, float a)
			{
				return a > b._clr.r && a > b._clr.g && a > b._clr.b;
			}
			public static bool operator <=(ColorOperator b, float a)
			{
				return a >= b._clr.r && a >= b._clr.g && a >= b._clr.b;
			}
			public static bool operator >(ColorOperator b, float a)
			{
				return a < b._clr.r && a < b._clr.g && a < b._clr.b;
			}
			public static bool operator >=(ColorOperator b, float a)
			{
				return a <= b._clr.r && a <= b._clr.g && a <= b._clr.b;
			}

			public static ColorOperator operator *(float a, ColorOperator b)
			{
				return new(a * b.Color);
			}
			public static ColorOperator operator /(float a, ColorOperator b)
			{
				return new(a / b._clr.r, a / b._clr.g, a / b._clr.b);
			}

			public static ColorOperator operator -(float a, ColorOperator b)
			{
				return new(a - b._clr.r, a - b._clr.g, a - b._clr.b);
			}
			public static ColorOperator operator -(ColorOperator a, float b)
			{
				return new(a._clr.r - b, a._clr.g - b, a._clr.b - b);
			}

			public static ColorOperator operator +(ColorOperator a, ColorOperator b)
			{
				return new(a._clr + b._clr);
			}

			public static ColorOperator operator -(ColorOperator a, ColorOperator b)
			{
				return new(a._clr - b._clr);
			}

			public static ColorOperator operator *(ColorOperator a, ColorOperator b)
			{
				return new(a._clr * b._clr);
			}

			public static ColorOperator operator /(ColorOperator a, ColorOperator b)
			{
				return new(a._clr.r / b._clr.r, a._clr.g / b._clr.g, a._clr.b / b._clr.b);
			}

			public static bool operator ==(ColorOperator a, Color b)
			{
				return a._clr == b;
			}
			public static bool operator !=(ColorOperator a, Color b)
			{
				return a._clr != b;
			}

			internal static ColorOperator Min(ColorOperator a, ColorOperator b)
			{
				return new(Mathf.Min(a._clr.r, b._clr.r), Mathf.Min(a._clr.g, b._clr.g), Mathf.Min(a._clr.b, b._clr.b));
			}

			internal static ColorOperator Max(ColorOperator a, ColorOperator b)
			{
				return new(Mathf.Max(a._clr.r, b._clr.r), Mathf.Max(a._clr.g, b._clr.g), Mathf.Max(a._clr.b, b._clr.b));
			}

			public override bool Equals(object obj)
			{
				return obj is ColorOperator op &&
					   _clr.Equals(op._clr);
			}

			public override int GetHashCode()
			{
				return 241020152 + _clr.GetHashCode();
			}

			internal static ColorOperator Sqrt(ColorOperator a)
			{
				return new(Mathf.Sqrt(a._clr.r), Mathf.Sqrt(a._clr.g), Mathf.Sqrt(a._clr.b));
			}
		}
	}
}
