using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevInterface;
using Menu;
using RegionKit.Extras.FutileExtras;
using static RegionKit.Extras.ColorTools;
using static RegionKit.Modules.PaletteEditor.DevPaletteEditor;
using static RegionKit.OptionsMenu.PaletteEditorTab;

namespace RegionKit.Modules.PaletteEditor
{
	// TODO: Move all Fsprite related things into this class, as well as the undos
	// TODO: Make sprite positioning based on the palette texture's anchor rather than hardcode it
	// TODO: Add a sprite to the color picker that shows a generic checker grid over the alpha with blend mode interpolation
	// TODO: Add terrain palette support

	public class Graphics
	{
		private static void AddSprite(FSprite sprite, List<FSprite>? list = null)
		{
			Futile.stage.AddChild(sprite);
			list?.Add(sprite);
		}
		private static void RemoveSprite(FSprite sprite, List<FSprite>? list = null)
		{
			sprite.RemoveFromContainer();
			list?.Remove(sprite);
		}

		private static void AddLabel(FLabel label, List<FLabel>? list = null)
		{
			Futile.stage.AddChild(label);
			list?.Add(label);
		}
		private static void RemoveLabel(FLabel label, List<FLabel>? list = null)
		{
			label.RemoveFromContainer();
			list?.Remove(label);
		}

		/// <summary>
		/// Parent class that hold the palette preview and all inner elements.
		/// </summary>
		internal class Preview
		{
			/// <summary>
			/// The exactly calculated pixel the mouse is currently hovering over.
			/// </summary>
			internal IntVector2 ExactHoveredPixel
			{
				get
				{
					return ClampIntVector(((Vector2)Futile.mousePosition - SpritePos - (new Vector2(Logic.PaletteScale, Logic.PaletteScale) / 2f)) / Logic.PaletteScaled(Terrain), false);
				}
			}

			/// <summary>
			/// The less precise pixel the mouse is currently hovering over.
			/// </summary>
			internal IntVector2 HoveredPixel
			{
				get
				{
					float mouseOffset = (Logic.PaletteScale * HoverPreview.BrushSize) / 2f;
					return ClampIntVector(((Vector2)Futile.mousePosition - new Vector2(mouseOffset, mouseOffset) - SpritePos) / Logic.PaletteScaled(Terrain), true);
				}
			}

			/// <summary>
			/// Indicatees whether the preview is for a terrain palette.
			/// </summary>
			public bool Terrain { get; }

			internal enum Anchor
			{
				Center,
				BottomLeft,
				BottomRight,
				TopLeft,
				TopRight
			}

			internal Anchor PaletteAnchor { get; }

			private Vector2 AnchorPoint => PaletteAnchor switch
			{
				Anchor.TopLeft => new(0f, 1f),
				Anchor.TopRight => new(1f, 1f),
				Anchor.BottomLeft => new(0f, 0f),
				Anchor.BottomRight => new(1f, 0f),
				Anchor.Center or _ => new(0.5f, 0.5f)
			};

			private Vector2 SpritePos
			{
				get
				{
					return PreviewTexture?.GetBottomLeftPos() ?? default;
				}
			}

			/// <summary>
			/// The mouse indicator for the currently hovered editing area.
			/// </summary>
			internal PixelSelector HoverPreview { get; }

			/// <summary>
			/// The visual indicators for unused keys in a palette.
			/// </summary>
			internal UnusedIndicator[]? UnusedKeys { get; }
			/// <summary>
			/// The visual dividers for key sections in a palette.
			/// </summary>
			public KeyLine[]? KeyLines { get; }
			public HoverToolTip? ToolTip { get; }

			/// <summary>
			/// The main texutre preview to view and edit.
			/// </summary>
			internal FTexture? PreviewTexture { get; }
			internal Texture2D? Texture
			{
				get => _texture;
				set
				{
					PreviewTexture?.SetTexture(value);
					_texture = value;
				}
			}
			private Texture2D? _texture;

			internal Preview(Texture2D? paletteTexture, Anchor anchor, bool terrain, DevUINode? owner = null)
			{
				Terrain = terrain;
				PaletteAnchor = anchor;

				if (paletteTexture != null)
				{
					Vector2 spriteAnchors = AnchorPoint;
					PreviewTexture = new(paletteTexture, "palettePreview")
					{
						scale = Logic.PaletteScale,
						anchorX = spriteAnchors.x,
						anchorY = spriteAnchors.y,
					};
					AddSprite(PreviewTexture, owner?.fSprites);
				}

				HoverPreview = new(owner);
				UnusedKeys = UnusedIndicator.GetUnusedIndicators(terrain, owner);
				KeyLines = KeyLine.GetKeyLines(terrain, owner);
				ToolTip = ModOptions.ShowKeyToolTip.Value ? new HoverToolTip() : null;
			}

			private IntVector2 ClampIntVector(Vector2 mouseLerp, bool brush)
			{
				Logic.GetImageSize(Terrain, out var imageSize);
				return new(ClampTilePositon(mouseLerp.x, imageSize.x, brush), ClampTilePositon(mouseLerp.y, imageSize.y, brush));
			}

			private int ClampTilePositon(float mouseLerp, int limit, bool brush)
			{
				return (int)Mathf.Max(0f, Mathf.Min(Mathf.Round(mouseLerp * limit), limit - (brush ? HoverPreview.BrushSize : 1)));
			}

			internal void GrafUpdate(bool mouseOver)
			{
				if (UnusedKeys != null)
				{
					foreach (UnusedIndicator key in UnusedKeys)
					{
						key.SetPos(SpritePos, Terrain);
					}
				}
				if (KeyLines != null)
				{
					foreach (KeyLine key in KeyLines)
					{
						key.SetPos(SpritePos);
					}
				}
			}

			internal void Update(bool mouseOver)
			{
				IntVector2 hoveredPixel = HoveredPixel;
				IntVector2 exactPixel = ExactHoveredPixel;
				HoverPreview?.Show(mouseOver);
				HoverPreview?.SetHoveredPixel(SpritePos, hoveredPixel, Terrain);

				Logic.GetImageSize(Terrain, out var imageSize);
				(Terrain ? Logic.TerrainPaletteKeys : Logic.RoomPaletteKeys)[exactPixel.x, imageSize.y - 1 - exactPixel.y].TryGet(out string key);
				ToolTip?.SetText(key);
				ToolTip?.Show(!string.IsNullOrEmpty(key) && mouseOver);
				ToolTip?.Update();

				if (UnusedKeys != null)
				{
					foreach (UnusedIndicator k in UnusedKeys)
					{
						k.SetPos(SpritePos, Terrain);
					}
				}
			}

			internal void SetScale()
			{
				if (PreviewTexture != null)
				{
					PreviewTexture.scale = Logic.PaletteScale;
				}

				HoverPreview?.Resize(HoverPreview.BrushSize * Logic.PaletteScale);

				if (UnusedKeys != null)
				{
					foreach (UnusedIndicator k in UnusedKeys)
					{
						k?.SetScale();
					}
				}

				if (KeyLines != null)
				{
					foreach (KeyLine k in KeyLines)
					{
						k?.SetScale();
					}
				}
				Update(false);
				GrafUpdate(false);
			}

			internal class PixelSelector : FRectangle
			{
				internal PixelSelector(DevUINode? owner = null) : base(new(Logic.PaletteScale, Logic.PaletteScale)) 
				{ 
					foreach (var sprite in Fsprites)
					{
						Futile.stage.AddChild(sprite);
						owner?.fSprites.Add(sprite);
					}
				}

				public int BrushSize
				{
					get => _brushSize;
					internal set
					{
						_brushSize = Math.Max(1, Math.Min(value, _maxBrushSize));
					}
				}
				private int _brushSize = 1;
				private readonly int _maxBrushSize = 6;

				internal void SetHoveredPixel(Vector2 spritePos, IntVector2 pixel, bool terrain)
				{
					Logic.GetImageSize(terrain, out var imageSize);
					Vector2 size = Logic.PaletteScaled(terrain);
					Vector2 pos = new(
						Mathf.Lerp(spritePos.x, spritePos.x + size.x, pixel.x / (float)imageSize.x),
						Mathf.Lerp(spritePos.y, spritePos.y + size.y, pixel.y / (float)imageSize.y));
					SetPosition(pos);

					foreach (FSprite line in Fsprites)
					{
						line.shader = rainWorld.Shaders["Inverted"];
					}
				}
			}

			internal class UnusedIndicator : FSprite
			{
				private DevUINode? _owner;
				private IntVector2 _key;

				internal UnusedIndicator(IntVector2 key, DevUINode? owner = null) : base("mouseEyeB5", false)
				{
					_owner = owner;
					_key = key;
					shader = rainWorld.Shaders["Inverted"];

					SetScale();

					Futile.stage.AddChild(this);
					owner?.fSprites.Add(this);
				}

				internal static UnusedIndicator[]? GetUnusedIndicators(bool terrain, DevUINode? owner = null)
				{
					if (!ModOptions.ShowUnusedKeyXs.Value) return null; // Don't create if they aren't needed

					Logic.Key[,] keys = terrain ? Logic.TerrainPaletteKeys : Logic.RoomPaletteKeys;
					List<UnusedIndicator> u = [];

					void AddIfUnused(int x, int y)
					{
						if (keys[x, y].unused) u.Add(new(new(x, y), owner));
					}
					Logic.IteratePixels(AddIfUnused, terrain);
					return [.. u];
				}

				internal void SetScale()
				{
					width = Logic.PaletteScale + 1f;
					height = Logic.PaletteScale + 1f;
				}

				internal void SetPos(Vector2 spritePos, bool terrain)
				{
					Logic.GetImageSize(terrain, out var imageSize); 
					Vector2 offset = new((_key.x * Logic.PaletteScale) + (Logic.PaletteScale / 2f), ((imageSize.y - 1 - _key.y) * Logic.PaletteScale) + (Logic.PaletteScale / 2f) + 0.5f);
					Vector2 pos = spritePos + offset;
					SetPosition(Mathf.Round(pos.x) + 0.001f, Mathf.Round(pos.y) + 0.001f);
				}

				internal void Remove()
				{
					RemoveSprite(this, _owner?.fSprites);
				}
			}

			/// <summary>
			/// Separator lines based on the hardcoded sections of the palette.
			/// </summary>
			internal class KeyLine : FSprite
			{
				private DevUINode? _owner;
				private bool _terrain;
				internal IntVector2 _initialPos; // Based on the bottom left of the pixel it resides on
				internal int _initialSize;
				internal bool _vertical;

				internal KeyLine(IntVector2 initialPos, int initialSize, bool vertical, bool terrain, DevUINode? owner = null) : base("pixel")
				{
					_initialPos = initialPos;
					_initialSize = initialSize;
					_vertical = vertical;
					_terrain = terrain;
					_owner = owner;

					SetScale();
					AddSprite(this, owner?.fSprites);
				}

				internal void SetScale()
				{
					if (_vertical)
					{
						scaleY = _initialSize * Logic.PaletteScale;
						anchorY = 0f;
					}
					else
					{
						scaleX = _initialSize * Logic.PaletteScale;
						anchorX = 0f;
					}
				}

				internal void SetPos(Vector2 pos)
				{
					Logic.GetImageSize(_terrain, out var imageSize);
					Vector2 offset = new(_initialPos.x * Logic.PaletteScale, ((imageSize.y - 1 - _initialPos.y) * Logic.PaletteScale) + 0.5f);
					SetPosition(pos + offset);
				}

				internal static KeyLine[]? GetKeyLines(bool terrain, DevUINode? owner = null)
				{
					if (!ModOptions.ShowKeyLines.Value) return null; // Don't create if not needed

					List<KeyLine> k = [];
					Logic.GetImageSize(terrain, out var imageSize);

					if (!terrain)
					{
						// Initial ones from the sun palette
						k = [
							// Top row
							new(new(2, 0), 1, true, terrain, owner),
							new(new(4, 0), 1, true, terrain, owner),
							new(new(9, 0), 1, true, terrain, owner),
							new(new(10, 0), 1, true, terrain, owner),
							new(new(13, 0), 1, true, terrain, owner),
							new(new(30, 0), 1, true, terrain, owner),
							new(new(31, 0), 1, true, terrain, owner),

							// Grime dividers
							new(new(0, 0), imageSize.x, false, terrain, owner),
							new(new(0, 1), imageSize.x, false, terrain, owner),

							// Sun / shade divider
							new(new(0, 4), imageSize.x, false, terrain, owner),

							// Sublayer dividers
							new(new(10, 7), 6, true, terrain, owner),
							new(new(20, 7), 6, true, terrain, owner),
						];

						// Then duplicate them
						int count = k.Count;
						for (int i = 0; i < count; i++)
						{
							k.Add(new(k[i]._initialPos + new IntVector2(0, 8), k[i]._initialSize, k[i]._vertical, terrain, owner));
						}

						k.Add(new(new(0, 7), imageSize.x, false, terrain, owner)); // Divider

						foreach (FSprite key in k)
						{
							key.isVisible = ModOptions.ShowKeyLines.Value;
						}

						k.AddRange([ // Surrounding boxes
							new(new(0, -1), imageSize.x, false, terrain, owner),
							new(new(0, 15), imageSize.y, true, terrain, owner),
							new(new(0, 15), imageSize.x, false, terrain, owner),
							new(new(imageSize.x, 15), imageSize.y, true, terrain, owner),
						]);
					}
					else // If terrain
					{
						// Color row
						int half = (imageSize.y / 2) - 1;
						k.Add(new(new(0, half), imageSize.x, false, terrain, owner)); // Sunlit divider

						int lastRow = imageSize.y - 1;
						k.Add(new(new(0, lastRow - 1), imageSize.x, false, terrain, owner)); // Divider

						k.AddRange([
							// Glitter
							new(new(1, lastRow), 1, true, terrain, owner),
							// Light tint (unused)
							new(new(2, lastRow), 1, true, terrain, owner),
							// Dust colors
							new(new(4, lastRow), 1, true, terrain, owner),
							// Sandstorm
							new(new(5, lastRow), 1, true, terrain, owner),
						]);

						k.AddRange([ // Surrounding boxes
							new(new(0, -1), imageSize.x, false, terrain, owner),
							new(new(0, imageSize.y - 1), imageSize.y, true, terrain, owner),
							new(new(0, imageSize.y - 1), imageSize.x, false, terrain, owner),
							new(new(imageSize.x, imageSize.y - 1), imageSize.y, true, terrain, owner),
						]);
					}

					return [.. k];
				}

				internal void Remove()
				{
					RemoveSprite(this, _owner?.fSprites);
				}
			}

			internal class HoverToolTip
			{
				private float _margin;
				private FLabel _label;
				private FSprite _box;
				private FRectangle _rect;
				private float _lastValidMouseX;

				internal HoverToolTip(DevUINode? owner = null)
				{
					_margin = NewPalettePage.Margin;
					_label = new FLabel(GetFont(), "") { anchorX = 0f, anchorY = 0f };
					_box = new FSprite("pixel")
					{
						anchorX = 0f,
						anchorY = 0f,
						color = owner != null ? owner.fSprites[0].color : MenuColorEffect.rgbDarkGrey,
						alpha = owner != null ? owner.fSprites[0].alpha : 0.5f,
						scaleX = _margin * 2f,
						scaleY = _label.FontLineHeight + (_margin * 2f)
					};
					_rect = new(new(_box.scaleX, _box.scaleY)); 
					foreach (var sprite in _rect.Fsprites)
					{
						Futile.stage.AddChild(sprite);
						owner?.fSprites.Add(sprite);
					}

					AddSprite(_box, owner?.fSprites);
					AddLabel(_label, owner?.fLabels);
				}

				internal void Update()
				{
					if (Futile.mousePosition.x < Custom.rainWorld.options.ScreenSize.x - _box.scaleX)
					{
						_lastValidMouseX = Futile.mousePosition.x + _box.scaleX;
					}
					_box.SetPosition(new(Mathf.Min(Futile.mousePosition.x, _lastValidMouseX - _box.scaleX), Mathf.Max(Futile.mousePosition.y, 0f)));
					_label.SetPosition(_box.GetPosition() + new Vector2(_margin + 0.001f, _margin));
					_rect.SetPosition(_box.GetPosition());

					_box.MoveToFront();
					_label.MoveToFront();
					_rect.MoveToFront();
				}

				internal void SetText(string text)
				{
					if (string.IsNullOrEmpty(text)) return;
					_label.text = text;
					_box.scaleX = _label.textRect.width + (_margin * 2f);
					_rect.Resize(_box.scaleX, _rect.Scale.y);
				}

				internal void Show(bool show)
				{
					_label.isVisible = show;
					_box.isVisible = show;
					_rect.Show(show);
				}
			}
		}
	}
}
