using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegionKit.Modules.PaletteEditor
{
	internal class Logic
	{
		/// <summary>
		/// The constant size of the palette image.
		/// </summary>
		internal static IntVector2 LevelImageSize = new(32, 16);
		/// <summary>
		/// The standard default size of a terrain palette.
		/// </summary>
		internal static IntVector2 TerrainImageSize = new(32, 7);
		internal static string DefaultTerrainPalette { get; } = "test_sand";
		internal static void GetImageSize(bool terrain, out IntVector2 size) => size = terrain ? TerrainImageSize : LevelImageSize;
		internal static void IteratePixels(Action<int, int> action, bool terrain)
		{
			GetImageSize(terrain, out IntVector2 imageSize);
			for (int x = 0; x < imageSize.x; x++)
				for (int y = 0; y < imageSize.y; y++)
					action(x, y);
		}
		/// <summary>
		/// The scale of the palette preview.
		/// </summary>
		internal static float PaletteScale { get => ModOptions.PaletteImageScale.Value; }
		/// <summary>
		/// Simplified equation to get the full image size.
		/// </summary>
		internal static Vector2 PaletteScaled(bool terrain)
		{
			GetImageSize(terrain, out IntVector2 imageSize);
			return PaletteScale * imageSize.ToVector2();
		}
		/// <summary>
		/// The current color to use when painting over the palette texture.
		/// </summary>


		/// <summary>
		/// A struct that holds various pixel information for palettes.
		/// </summary>
		/// <param name="key">The hardcoded purpose of the color for various palette dependant thing.</param>
		internal struct Key(string key)
		{
			private readonly string _key = key;
			internal bool unused;

			internal readonly bool TryGet(out string key)
			{
				key = _key;
				return !string.IsNullOrEmpty(key);
			}

			internal static Key[,] InitRoomPalette(RoomSettings? settings = null)
			{
				Key[,] keys = new Key[LevelImageSize.x, LevelImageSize.y];
				string[,] keyNames = new string[LevelImageSize.x, LevelImageSize.y];

				keyNames[0, 0] = "Sky";
				keyNames[1, 0] = "Fog";
				keyNames[2, 0] = "Black";
				keyNames[3, 0] = "Item";
				keyNames[4, 0] = "Deep Water Top";
				keyNames[5, 0] = "Deep Water Bottom";
				keyNames[6, 0] = "Water Surface Close";
				keyNames[7, 0] = "Water Surface Far";
				keyNames[8, 0] = "Water Surface Highlight";
				keyNames[9, 0] = "Fog Intensity";
				keyNames[10, 0] = "Shortcut Dot";
				keyNames[11, 0] = "Shortcut Dot Blink";
				keyNames[12, 0] = "Shortcut Dot Travel";
				keyNames[13, 0] = "Shortcut Symbol";
				keyNames[30, 0] = "Darkness";

				for (int x = 0; x < LevelImageSize.x; x++)
				{
					keyNames[x, 1] = "Grime";
					if (x < 30)
					{
						keyNames[x, 2] = $"Sun Highlight [{x}]";
						keyNames[x, 3] = $"Sun Middle [{x}]";
						keyNames[x, 4] = $"Sun Shadow [{x}]";
						keyNames[x, 5] = $"Shade Highlight [{x}]";
						keyNames[x, 6] = $"Shade Middle [{x}]";
						keyNames[x, 7] = $"Shade Shadow [{x}]";
					}
					else
					{
						bool saved = settings != null && settings.EffectColorA == -2;
						string notSaved = saved ? "" : "(Not Saved)";
						keyNames[x, 2] = $"Effect Color A {notSaved}";
						keyNames[x, 3] = $"Effect Color A {notSaved}";

						saved = settings != null && settings.EffectColorB == -2;
						keyNames[x, 4] = $"Effect Color B {notSaved}";
						keyNames[x, 5] = $"Effect Color B {notSaved}";
					}
				}

				for (int x = 0; x < LevelImageSize.x; x++)
				{
					for (int y = 0; y < LevelImageSize.y / 2; y++)
					{
						keys[x, y] = new(keyNames[x, y]);
						if (!string.IsNullOrEmpty(keyNames[x, y]))
							keys[x, y + 8] = new($"(Rain) {keyNames[x, y]}");
						if (string.IsNullOrEmpty(keyNames[x, y]) || keyNames[x, y].Contains("Not Saved"))
						{
							keys[x, y].unused = true;
							keys[x, y + 8].unused = true;
						}
					}
				}

				return keys;
			}

			/// <summary>
			/// A dynamically updating key generator, since terrain palettes are variable.
			/// </summary>
			internal static Key[,] InitTerrainPalette(int sizeX, int sizeY)
			{
				TerrainImageSize = new(sizeX, sizeY);
				Key[,] keys = new Key[sizeX, sizeY];
				string[,] keyNames = new string[sizeX, sizeY];

				// Constant values
				int lastRow = sizeY - 1;
				keyNames[0, lastRow] = "Glitter";
				keyNames[1, lastRow] = "Light Tint (Unused)";
				keyNames[2, lastRow] = "Dark Dust";
				keyNames[3, lastRow] = "Light Dust";
				keyNames[4, lastRow] = "Sandstorm";

				for (int x = 0; x < sizeX; x++)
				{
					// Half of the length since we dynamically create the layers
					int yLength = Math.Min(1, (sizeY - 1) / 2);
					for (int y = 0; y < yLength; y++)
					{
						string layer = y == 0 ? "Background" : y == yLength - 1 ? "Foreground" : "Midground";
						// TODO: Examine what the x vs y axis of a terrain palette affects in-game
						keyNames[x, y] = $"Sun {layer} [{x}]";
						keyNames[x, ((y + 1) * 2) - 1] = $"Shade {layer} [{x}]";
					}
				}

				for (int x = 0; x < sizeX; x++)
				{
					for (int y = 0; y < sizeY; y++)
					{
						keys[x, y] = new(keyNames[x, y]);
						if (y == sizeY - 1 && (string.IsNullOrEmpty(keyNames[x, y]) || keyNames[x, y].Contains("Unused")))
							keys[x, y].unused = true;
					}
				}
				// Light Tint is unused in base Watcher
				keys[1, lastRow].unused = true;
				return keys;
			}
		}
		/// <summary>
		/// Constant palette values to use for reference for the level render.
		/// </summary>
		internal static Key[,] RoomPaletteKeys { get; } = Key.InitRoomPalette();
		/// <summary>
		/// Constant palette values to use for reference for the terrain object.
		/// </summary>
		internal static Key[,] TerrainPaletteKeys { get; set; } = Key.InitTerrainPalette(TerrainImageSize.x, TerrainImageSize.y);
	}
}
