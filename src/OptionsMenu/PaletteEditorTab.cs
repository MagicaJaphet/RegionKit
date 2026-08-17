using System.IO;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using RegionKit.Modules.PaletteEditor;
using static RegionKit.ModOptions;
using static RegionKit.Modules.PaletteEditor.Graphics;

namespace RegionKit.OptionsMenu
{
	/// <summary>
	/// Remix Options for the Palette Editor by MagicaJaphet.
	/// </summary>
	internal class PaletteEditorTab : OpTab
	{
		private Vector2 _nextItemPos;

		public static float Margin { get; } = 10f;
		public PaletteEditorTab(OptionInterface owner) : base(owner, "Palette Editor")
		{
		}

		public void Initialize()
		{
			_nextItemPos = new(Margin, CanvasSize.y - (Margin * 3f));

				AddItem(new OpDragger(UndoStack, _nextItemPos) { max = 100, min = 0 });
			AddItem(new OpCheckBox(LoadSavedPalettes, _nextItemPos));
			AddItem(new PaletteScaleSlider(PaletteImageScale, _nextItemPos, 100, 0)
			{
				max = 15f,
				min = 8f,
				_increment = 50,
			});
			AddItem(new KeyLineCheckBox(ShowKeyLines, _nextItemPos));
			AddItem(new KeyToolTipButton(ShowKeyToolTip, _nextItemPos));
			AddItem(new UnusedKeysButton(ShowUnusedKeyXs, _nextItemPos));

			AddItems(new PalettePreviewer(_container.GetPosition() + new Vector2(CanvasSize.x - Margin, Margin)));
		}

		internal void Update()
		{
		}

		internal void AddItem(UIconfig item)
		{
			_nextItemPos = item.pos + new Vector2(0f, -(item.size.y + Margin));

			item.description = item.cfgEntry.info.description;
			OpLabel label = new(item.pos.x + item.size.x + Margin, item.pos.y, item.cfgEntry.info.Tags.FirstOrDefault()?.ToString() ?? "<MISSING>");

			AddItems(item, label);
		}

		internal class UnusedKeysButton : OpCheckBox
		{
			public UnusedKeysButton(Configurable<bool> config, Vector2 pos) : base(config, pos)
			{
			}

			public override void Change()
			{
				base.Change();

				//if (PaletteScaleSlider._xLines != null)
				//{
				//	foreach (var x in PaletteScaleSlider._xLines)
				//	{
				//		x.forceHide = !this.GetValueBool();
				//		x.Show(this.GetValueBool());
				//	}
				//}
			}
		}

		internal class KeyToolTipButton : OpCheckBox
		{
			public KeyToolTipButton(Configurable<bool> config, Vector2 pos) : base(config, pos)
			{
			}

			public override void Change()
			{
				base.Change();

				//if (PaletteScaleSlider._hoverKey != null)
				//{
				//	PaletteScaleSlider._hoverKey.forceHide = !this.GetValueBool();
				//}
			}
		}


		internal class KeyLineCheckBox : OpCheckBox
		{
			public KeyLineCheckBox(Configurable<bool> config, Vector2 pos) : base(config, pos)
			{
			}

			public override void Change()
			{
				base.Change();
				if (PalettePreviewer._preview?.KeyLines != null)
				{
					bool value = this.GetValueBool();
					for (int i = 0; i < PalettePreviewer._preview.KeyLines.Length; i++)
					{
						PalettePreviewer._preview.KeyLines[i].isVisible = value;
					}
				}
			}
		}

		internal class PaletteScaleSlider : OpFloatSlider
		{

			public PaletteScaleSlider(Configurable<float> config, Vector2 pos, int length, byte decimalNum = 1, bool vertical = false) : base(config, pos, length, decimalNum, vertical)
			{
			}

			public override void Update()
			{
				base.Update();

				float scale = this.GetValueFloat();
			}

			public override void Change()
			{
				base.Change();

				float scale = this.GetValueFloat();
				PaletteImageScale.Value = scale;
				PalettePreviewer._preview?.SetScale();
			}
		}

		internal class PalettePreviewer : UIelement
		{
			internal static Preview? _preview;
			private Texture2D _outskirtsPalette;
			private IntVector2 _imageSize;

			public PalettePreviewer(Vector2 imagePos) : base(imagePos, new Vector2())
			{
				if (_outskirtsPalette == null)
				{
					Logic.GetImageSize(false, out _imageSize);
					_outskirtsPalette = new Texture2D(_imageSize.x, _imageSize.y, TextureFormat.ARGB32, false);
					try
					{
						AssetManager.SafeWWWLoadTexture(ref _outskirtsPalette, "file:///" + AssetManager.ResolveFilePath(Path.Combine("Palettes", "palette0.png")), false, true);
					}
					catch (FileLoadException) { }
				}
				_preview = new(_outskirtsPalette, Preview.Anchor.BottomRight, false);
				_preview.PreviewTexture?.SetPosition(imagePos);
			}

			public override void Update()
			{
				Vector2 _imagePosition = _preview?.PreviewTexture?.GetBottomLeftPos() ?? new();
				Vector2 mousePos = Futile.mousePosition;
				bool mouseOverPreview = mousePos.x > _imagePosition.x && mousePos.x < _imagePosition.x + _imageSize.x * PaletteImageScale.Value
					&& mousePos.y > _imagePosition.y && mousePos.y < _imagePosition.y + _imageSize.y * PaletteImageScale.Value;
				_preview?.GrafUpdate(mouseOverPreview);
				_preview?.Update(mouseOverPreview);

				base.Update();
			}
		}
	}
}
