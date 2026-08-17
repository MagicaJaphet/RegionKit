using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RegionKit.Extras.ColorTools;

namespace RegionKit.Modules.PaletteEditor
{
	public class Editor
	{
		internal static Color PaintColor
		{
			get
			{
				return CurrentColor == 0 ? _mainPaintColor : _secondaryPaintColor;
			}
			set
			{
				if (CurrentColor == 0)
				{
					_mainPaintColor = value;
				}
				else
				{
					_secondaryPaintColor = value;
				}
			}
		}
		/// <summary>
		/// The current main color to use when painting over the palette texture.
		/// </summary>
		internal static Color _mainPaintColor = Color.red;
		/// <summary>
		/// The current secondary color to use when painting over the palette texture.
		/// </summary>
		internal static Color _secondaryPaintColor = Color.blue;

		/// <summary>
		/// The color to be used for calculations.
		/// </summary>
		internal static int CurrentColor { get; set; }

		/// <summary>
		/// Enums for various blend modes, found in most art programs.
		/// </summary>
		internal enum BlendMode
		{
			Normal,
			Multiply,
			Screen,
			Overlay,
			HardLight,
			SoftLight,
			ColorDodge,
			Burn,
			Divide,
			Add,
			Darken,
			Lighten,
			Hue
		}
		/// <summary>
		/// The current blend mode to use for color calculations.
		/// </summary>
		internal static BlendMode CurrentBlendMode { get; set; } = BlendMode.Normal;

		/// <summary>
		/// A history record for states and changes of a palette texture.
		/// </summary>
		internal class UndoablePalette
		{
			internal Texture2D? Texture
			{
				get => _texture;
				set
				{
					_texture = value;
					Clear();
				}
			}
			private Texture2D? _texture;

			private int _index;
			private static int UndoStack { get => ModOptions.UndoStack.Value; }
			private readonly List<Color[]> _stack = [];
			private Color[]? _init;
			internal static readonly int _colorHistory = 8;
			internal Color[] _history = new Color[_colorHistory];
			internal static FSprite[] _historySprites = new FSprite[_colorHistory];

			internal bool CanUndo
			{
				get
				{
					return _index + 1 < _stack.Count;
				}
			}

			internal bool CanRedo
			{
				get
				{
					return _index - 1 >= 0;
				}
			}

			public int PalIndex { get; }
			public bool Terrain { get => PalIndex == -1; }
			public Graphics.Preview? PalettePreview { get; set; }

			internal UndoablePalette(int index) => PalIndex = index;

			private void UpdateHistory(int index, float alpha, Color color)
			{
				_history[index] = color;
				_history[index].a = alpha;
				if (_historySprites.Length > index && _historySprites[index] != null)
				{
					_historySprites[index].color = color;
					_historySprites[index].alpha = alpha;
				}
			}

			internal void Paint(int x, int y, bool blend = true)
			{
				if (Texture == null) return;
				ColorOperator a = new(Texture.GetPixel(x, y)); // base layer
				ColorOperator b = new(PaintColor); // top layer

				// Blend mode calculations
				if (blend)
				{
					Color equal = b.Color;
					equal.a = PaintColor.a;
					if (_history[0] != equal)
					{
						for (int i = _history.Length - 1; i >= 1; i--)
						{
							var lastState = _history[i - 1];
							UpdateHistory(i, lastState.a, lastState);
						}

						UpdateHistory(0, PaintColor.a, b.Color);
					}

					switch (CurrentBlendMode)
					{
						case BlendMode.Normal:
							break;

						case BlendMode.Multiply:
							b *= a;
							break;

						case BlendMode.Screen:
							b = (a.Inverted * b.Inverted).Inverted;
							break;

						case BlendMode.Overlay:
							if (a < 0.5f)
							{
								b = 2f * a * b;
							}
							else
							{
								b = (2f * a.Inverted * b.Inverted).Inverted;
							}
							break;

						case BlendMode.HardLight:
							if (b < 0.5f)
							{
								b = 2f * a * b;
							}
							else
							{
								b = (2f * a.Inverted * b.Inverted).Inverted;
							}
							break;

						case BlendMode.SoftLight:
							if (b <= 0.5f)
							{
								b = a - ((2f * b).Inverted * a * a.Inverted);
							}
							else
							{
								ColorOperator g = a <= 0.25f ?
									((((16 * a) - 12f) * a) + 4f) * a
									: ColorOperator.Sqrt(a);

								b = a + (((2f * b) - 1f) * (g - a));
							}
							break;

						case BlendMode.ColorDodge:
							b = a / b.Inverted;
							break;

						case BlendMode.Burn:
							b = (b.Inverted / a).Inverted;
							break;

						case BlendMode.Divide:
							b = a / b;
							break;

						case BlendMode.Add:
							b += a;
							break;

						case BlendMode.Darken:
							b = ColorOperator.Min(a, b);
							break;

						case BlendMode.Lighten:
							b = ColorOperator.Max(a, b);
							break;

						case BlendMode.Hue:
							Color.RGBToHSV(a.Color, out var aH, out var aS, out var aV);
							Color.RGBToHSV(b.Color, out var bH, out var bS, out var bV);
							b = new(Color.HSVToRGB(bH, aS, aV));
							break;
					}
				}

				Texture.SetPixel(x, y, Color.Lerp(a.Color, b.Color, PaintColor.a));
			}

			internal void Paint(Color[]? cols)
			{
				if (cols == null || Texture == null || cols?.Length != Texture.GetPixels().Length) return;
				Texture.SetPixels(cols);
			}

			internal void PickColor(int x, int y)
			{
				if (Texture == null) return;
				PaintColor = Texture.GetPixel(x, y);
			}

			internal void Apply(RoomCamera rCam)
			{
				if (Texture == null) return;
				Texture.Apply();
				if (PalettePreview != null)
					PalettePreview.Texture = Texture;

				if (Terrain)
				{
					if (rCam.room != null)
					{
						string test = Logic.DefaultTerrainPalette;
						string name = rCam.room.roomSettings.terrainPalette ?? test;
						if (name == "NO PALETTE") name = test;
						rCam.terrainPalette ??= new(name, null);
						rCam.terrainPalette.mainPal.main = Texture.GetPixels();

						float fade = 0f;
						RoomSettings.TerrainFadePalette terrainFadePalette = rCam.room.roomSettings.terrainFadePalette;
						if (terrainFadePalette != null && rCam.currentCameraPosition >= 0 && rCam.currentCameraPosition < terrainFadePalette.fades.Length)
						{
							fade = rCam.room.roomSettings.terrainFadePalette.fades[rCam.currentCameraPosition];
						}
						rCam.terrainPalette.UpdateFade(fade, rCam.mushroomMode, rCam.DarkPalette, rCam.ghostMode, rCam.rotMode);
					}
				}
				else
				{
					rCam?.ApplyFade();
				}
			}

			internal void Reset(RoomCamera rCam)
			{
				if (Texture == null || _init == null) return;
				Texture.SetPixels(_init);
				Apply(rCam);
				Clear();
				AddToStack();
			}

			internal void Init()
			{
				if (Texture == null || _stack.Count > 0) return;
				_init = Texture.GetPixels();
				AddToStack();
			}

			internal void AddToStack()
			{
				if (Texture == null) return;

				if (_index != 0)
				{
					_stack.RemoveRange(0, Math.Min(_index, _stack.Count - 1));
					_index = 0;
				}
				_stack.Insert(0, Texture.GetPixels());
				if (_stack.Count > UndoStack)
				{
					_stack.RemoveRange(UndoStack, _stack.Count - UndoStack);
				}
			}

			internal void Undo(RoomCamera rCam)
			{
				if (CanUndo)
				{
					Paint(GetStack(ClampIndex(++_index)));
					Apply(rCam);
				}
			}

			internal void Redo(RoomCamera rCam)
			{
				if (CanRedo)
				{
					Paint(GetStack(ClampIndex(--_index)));
					Apply(rCam);
				}
			}

			private void Clear()
			{
				_stack.Clear();
				_index = 0;
			}

			private Color[]? GetStack(int index)
			{
				if (index >= 0 && index < _stack.Count)
					return _stack[index];
				return null;
			}

			private int ClampIndex(int index)
			{
				return Math.Max(0, Math.Min(index, UndoStack - 1));
			}

			internal void CopySunToRain(RoomCamera rCam)
			{
				if (Texture == null) return;
				Logic.GetImageSize(Terrain, out var imageSize);
				for (int x = 0; x < imageSize.x; x++)
				{
					int halfHeight = Terrain ? (imageSize.y - 1) / 2 : 8;
					for (int y = Terrain ? imageSize.y - 1 : 15; y >= halfHeight; y--) // Texture2D index from the bottom left, so start at the top
					{
						Texture.SetPixel(x, y - halfHeight, Texture.GetPixel(x, y));
					}
				}
				AddToStack();
				Apply(rCam);
			}
		}
		/// <summary>
		/// Used for the main palette.
		/// </summary>
		internal static UndoablePalette MainPalette { get; } = new(0);
		/// <summary>
		/// Used for the fade palette.
		/// </summary>
		internal static UndoablePalette FadePalette { get; } = new(1);
		/// <summary>
		/// Used for the terrain palette.
		/// </summary>
		internal static UndoablePalette TerrainPalette { get; } = new(-1);
		/// <summary>
		/// Used for the extended fade palettes that RegionKit allows use of.
		/// </summary>
		internal static List<UndoablePalette> MoreFadePalettes { get; } = [];
	}
	
}
