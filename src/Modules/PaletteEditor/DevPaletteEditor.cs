using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevInterface;
using MoreSlugcats;
using RegionKit.Extras;
using RegionKit.Modules.DevUIMisc;
using RegionKit.Modules.DevUIMisc.GenericNodes;
using static RegionKit.Modules.PaletteEditor.Graphics;

namespace RegionKit.Modules.PaletteEditor
{
	internal class DevPaletteEditor
	{
		//internal class PalettePage : DevInterface.Page
		//{
		//	private SmallElements.BlendModeButton _blendModeButton;

		/// <summary>
		/// Sets up the UI for the palette editor.
		/// </summary>
		//	internal PalettePage(DevUI owner) : base(owner, "Palette_Editor_Page", null, Name)
		//	{
		//		Panel palettePreview = new(owner, "Palette_Image", this, new(Custom.rainWorld.options.ScreenSize.x - PalettePanelSize(false).x - Margin - Padding.x, Margin), PalettePanelSize(false) + Padding, "Palette Image");
		//		subNodes.Add(palettePreview);
		//		palettePreview.subNodes.Add(new PaletteEditor(owner, palettePreview, Editor.MainPalette));
		//		palettePreview.subNodes.Add(new SmallElements.ReloadButton(owner, palettePreview));
		//		palettePreview.subNodes.Add(new SmallElements.SaveButton(owner, palettePreview));
		//		for (int i = 0; i < 2 + (RegionKitWrapper.RegionKitEnabled ? Editor.MoreFadePalettes.Count : 0); i++)
		//		{
		//			palettePreview.subNodes.Add(new SmallElements.PaletteButton(owner, palettePreview, i));
		//		}

		//		Panel colorPicker = new(owner, "Color_Picker", this, palettePreview.absPos - new Vector2(200f, 0f), new(190f, 205f), "Color Picker");
		//		subNodes.Add(colorPicker);
		//		colorPicker.subNodes.Add(new ColorPicker(owner, colorPicker));
		//		colorPicker.subNodes.Add(new SmallElements.AlphaSlider(owner, colorPicker));
		//		_blendModeButton = new SmallElements.BlendModeButton(owner, colorPicker);
		//		colorPicker.subNodes.Add(_blendModeButton);
		//		for (int i = 0; i < Editor.UndoablePalette._colorHistory; i++)
		//		{
		//			colorPicker.subNodes.Add(new SmallElements.ColorHistory(owner, colorPicker, i));
		//		}

		//		Panel _tempSettingsPanel = new Panel(owner, "Temp_Settings", this, new(20f, 20f), new(200f, 70f), "Preview Settings");
		//		subNodes.Add(_tempSettingsPanel);
		//		_tempSettingsPanel.subNodes.Add(new SmallElements.RainSlider(owner, _tempSettingsPanel));
		//		_tempSettingsPanel.subNodes.Add(new SmallElements.WaterButton(owner, _tempSettingsPanel));
		//		_tempSettingsPanel.subNodes.Add(new SmallElements.WaterSlider(owner, _tempSettingsPanel));
		//	}

		//	public override void Signal(DevUISignalType type, DevUINode sender, string message)
		//	{
		//		if (sender == _blendModeButton)
		//		{
		//			Editor.CurrentBlendMode = (Editor.BlendMode)Enum.Parse(typeof(Editor.BlendMode), message);
		//			_blendModeButton.Text = message;
		//		}
		//	}

		//	internal class SmallElements
		//	{

		//		internal class ColorHistory : RectangularDevUINode
		//		{
		//			private int _index;
		//			private FSprite[] _boxLines;

		//			internal ColorHistory(DevUI owner, Panel parentNode, int index) : base(owner, "Color_History", parentNode, new(160f, 205f - (GenericElementHeight + Margin + ((GenericElementHeight + Margin) * index))), new(25f, GenericElementHeight))
		//			{
		//				_index = index;

		//				Editor.UndoablePalette._historySprites[index] = new("pixel")
		//				{
		//					width = size.x,
		//					height = size.y,
		//					anchorX = 0,
		//					anchorY = 0,
		//				};
		//				Futile.stage.AddChild(Editor.UndoablePalette._historySprites[index]);
		//				fSprites.Add(Editor.UndoablePalette._historySprites[index]);

		//				if (SelectedPalette != null && SelectedPalette._history[index] != null)
		//				{
		//					Color col = SelectedPalette._history[index];
		//					Editor.UndoablePalette._historySprites[index].color = col;
		//					Editor.UndoablePalette._historySprites[index].alpha = col.a;
		//				}

		//				_boxLines = [
		//					new("pixel") { scaleX = size.x, anchorX = 0 },
		//					new("pixel") { scaleY = size.y + 0.5f, anchorY = 0 },
		//					new("pixel") { scaleX = size.x, anchorX = 0 },
		//					new("pixel") { scaleY = size.y, anchorY = 0 }
		//					];
		//				foreach (var b in _boxLines)
		//				{
		//					Futile.stage.AddChild(b);
		//					fSprites.Add(b);
		//				}
		//			}

		//			public override void Update()
		//			{
		//				base.Update();
		//				if (MouseOver && !WasClicked && Input.GetMouseButtonDown(0))
		//				{
		//					Editor.PaintColor.Value = Editor.UndoablePalette._historySprites[_index].color;
		//					Editor.Alpha = Editor.UndoablePalette._historySprites[_index].alpha;
		//				}
		//			}

		//			public override void Refresh()
		//			{
		//				base.Refresh();
		//				MoveSprite(0, absPos);
		//				for (int i = 0; i < _boxLines.Length; i++)
		//				{
		//					MoveSprite(i + 1, absPos +
		//						i switch
		//						{
		//							1 => new Vector2(0f, -0.5f),
		//							2 => new Vector2(0f, size.y),
		//							3 => new Vector2(size.x, 0f),
		//							_ => new Vector2()
		//						});
		//				}
		//			}
		//		}

		


		//		internal class AlphaSlider : DevInterface.Slider
		//		{
		//			private float _lastAlpha;

		//			internal AlphaSlider(DevUI owner, Panel parentNode) : base(owner, "Color_Alpha", parentNode, new(Margin, (Margin * 2f) + GenericElementHeight), "Alpha", false, 20f) { }

		//			public override void NubDragged(float nubPos)
		//			{
		//				Editor.Alpha = nubPos;
		//				parentNode.Refresh();
		//				Refresh();
		//			}

		//			public override void Update()
		//			{
		//				base.Update();

		//				if (_lastAlpha != Editor.Alpha)
		//				{
		//					_lastAlpha = Editor.Alpha;
		//					RefreshNubPos(Editor.Alpha);
		//				}
		//			}

		//			public override void Refresh()
		//			{
		//				base.Refresh();
		//				NumberText = "";
		//				RefreshNubPos(Editor.Alpha);
		//			}
		//		}

		//		internal class BlendModeButton : ButtonWithSelectPanel
		//		{
		//			internal static SelectPanel MakePanel(ButtonWithSelectPanel button)
		//			{
		//				string[] modes = Enum.GetNames(typeof(Editor.BlendMode));
		//				return new SelectPanel(button.owner, "Blend_Mode_Select", button, new(button.pos.x - 150f, Margin), new(155f, (Margin * 2f) + (GenericElementHeight * modes.Length)), "Blend Modes", modes);
		//			}

		//			internal BlendModeButton(DevUI owner, Panel parentNode) : base(owner, "Blend_Mode", parentNode, new(Margin, Margin), 150f, Enum.GetName(typeof(Editor.BlendMode), Editor.CurrentBlendMode), new MakeSelectPanel(MakePanel)) { }
		//		}

		//		internal class PaletteButton : Button
		//		{
		//			private int _index;
		//			private static float normalWidth = 30f;
		//			private static float shortWidth = 15f;

		//			public PaletteButton(DevUI owner, Panel parentNode, int index) : 
		//				base(owner, $"Palette_Selector{index}", parentNode, 
		//					new(Margin + ((RegionKitWrapper.RegionKitEnabled && Editor.MoreFadePalettes.Count > 0 && index > 1 ? ((shortWidth + Margin) * (index - 1)) + (normalWidth + Margin) : (normalWidth + Margin) * index)), (Margin * 2f) + GenericElementHeight), 
		//					RegionKitWrapper.RegionKitEnabled && Editor.MoreFadePalettes.Count > 0 && index > 0 ? shortWidth : normalWidth, 
		//					index == 0 ? "Main" : RegionKitWrapper.RegionKitEnabled && Editor.MoreFadePalettes.Count > 0 ? $"F{index}" : "Fade")
		//			{
		//				_index = index;
		//			}

		//			public override void Clicked()
		//			{
		//				Refresh();
		//				base.Clicked();
		//				SelectedPalette = _index switch
		//				{
		//					0 => Editor.MainPalette,
		//					1 => Editor.FadePalette,
		//					_ => Editor.MoreFadePalettes.FirstOrDefault(x => x.PalIndex == _index) ?? Editor.MainPalette
		//				};
		//				for (int i = 0; i < Editor.UndoablePalette._colorHistory; i++)
		//				{
		//					Editor.UndoablePalette._historySprites[i].color = SelectedPalette._history[i];
		//					Editor.UndoablePalette._historySprites[i].alpha = SelectedPalette._history[i].a;
		//				}
		//			}
		//		}

		//		internal class ReloadButton(DevUI owner, Panel parentNode) : Button(owner, $"Reload_Palette", parentNode, new(parentNode.size.x - Margin - width, Margin), width, "Reload")
		//		{
		//			internal static float width = 45f;

		//			public override void Clicked()
		//			{
		//				base.Clicked();
		//				SelectedPalette?.Reset(owner.game.cameras[0]);
		//			}
		//		}

		//		internal class SaveButton : Button
		//		{
		//			private bool _saving;
		//			internal static float width = 65f;

		//			internal static string SavePath { get; } = Path.Combine(Application.streamingAssetsPath, "savedpalettes");

		//			public SaveButton(DevUI owner, Panel parentNode) : base(owner, $"Save_Palette", parentNode, new(parentNode.size.x - (Margin * 2f) - ReloadButton.width - width, Margin), width, "Save Image")
		//			{
		//			}

		//			public override void Clicked()
		//			{
		//				base.Clicked();

		//				if (_saving) return;
		//				_saving = true;

		//				try
		//				{
		//					if (!Directory.Exists(SavePath) && Directory.Exists(Application.streamingAssetsPath))
		//					{
		//						Directory.CreateDirectory(SavePath);
		//					}

		//					if (SelectedPalette?.Texture != null)
		//					{
		//						Texture2D cloneWithoutEffectCols = SelectedPalette.Texture.Clone();

		//						// Check if save effect colors option is present and applicable
		//						if (!RegionKitWrapper.RegionKitEnabled || RegionKitWrapper.PaletteEffectColorActive(owner.room.roomSettings))
		//						{
		//							Logic.GetImageSize(false, out var imageSize);
		//							for (int x = 30; x < imageSize.x; x++)
		//							{
		//								for (int y = 0; y < 14; y++)
		//								{
		//									if (y == 6 || y == 7) continue;
		//									cloneWithoutEffectCols.SetPixel(x, y, Color.white);
		//								}
		//							}
		//						}
		//						cloneWithoutEffectCols.Apply();
		//						File.WriteAllBytes(Path.Combine(SavePath, $"palette{(SelectedPalette.PalIndex switch
		//						{
		//							0 => owner.room.roomSettings.pal ?? -1,
		//							1 => owner.room.roomSettings.fadePalette.palette,
		//							_ => RegionKitWrapper.RegionKitEnabled ? RegionKitWrapper.GetPalNumber(owner.room.roomSettings, SelectedPalette.PalIndex) : -1
		//						})}.png"), cloneWithoutEffectCols.EncodeToPNG());
		//					}
		//				}
		//				catch (Exception ex)
		//				{
		//					Plugin.Logger.LogError(ex);
		//				}

		//				_saving = false;
		//			}
		//		}
		//	}

		//	internal class ColorPicker : DevUINode
		//	{
		//		internal class PaletteEditorMenuWrapper : Menu.Menu
		//		{
		//			internal PaletteEditorMenuWrapper() : base(Custom.rainWorld.processManager, null) => pages = [new(this, null, "Page", 0)];
		//		}

		//		private PaletteEditorMenuWrapper _menu = new();
		//		private MenuTabWrapper _tab;
		//		private UIelementWrapper _wrapper;
		//		private OpColorPicker _colorPicker;
		//		private Color _lastColorValue;
		//		private static Vector2 _offset = new (Margin, (GenericElementHeight * 2f) + (Margin * 2f));

		//		internal ColorPicker(DevUI owner, Panel parentNode) : base(owner, "Color_Picker_Element", parentNode)
		//		{
		//			_tab = new(_menu, _menu.pages[0]);
		//			_colorPicker = new OpColorPicker(Editor.PaintColor, parentNode.absPos + _offset);
		//			_wrapper = new(_tab, _colorPicker);
		//		}

		//		public override void Update()
		//		{
		//			base.Update();

		//			if (!WasClicked)
		//			{
		//				if (_lastColorValue != Editor.PaintColor.Value)
		//				{
		//					_lastColorValue = Editor.PaintColor.Value;
		//					_colorPicker.valueColor = _lastColorValue;
		//				}
		//				_menu?.Update();
		//				_tab?.Update();
		//				_wrapper?.Update();
		//				_colorPicker?.Update();
		//				if (_colorPicker != null)
		//					Editor.PaintColor.Value = _colorPicker.valueColor;
		//			}
		//		}

		//		public override void Refresh()
		//		{
		//			base.Refresh();

		//			_colorPicker?.SetPos((parentNode as Panel).absPos + _offset);
		//			_colorPicker?.GrafUpdate(Custom.rainWorld.processManager.currentMainLoop.myTimeStacker);
		//			if (_colorPicker != null)
		//				_colorPicker._cdis0.alpha = Editor.Alpha;
		//		}

		//		public override void ClearSprites()
		//		{
		//			base.ClearSprites();
		//			_colorPicker?.Unload();
		//		}
		//	}
		//}

		internal class NewPalettePage : Page
		{
			// TODO: Nest level specific buttons in the palette preview editor
			// TODO: Redo logic to separate graphics from editor more
			public static string Name { get; } = "Palette Editor";
			public static string ID { get; } = "Palette_Editor_Page";
			public static Vector2 ScreenSize { get => rainWorld.options.ScreenSize; }

			// Size and positioning related properties
			internal static float Margin { get; } = 5f;
			internal static Vector2 Padding { get; } = new(Margin * 2f, Margin * 2f);
			internal static float GenericElementHeight { get; } = 20f;
			internal static float GenericPanelWidth { get; } = 200f;
			public NewPalettePage(DevUI owner) : base(owner, ID, null, Name)
			{
				// Initialize palette histories
				Editor.MainPalette.Init();
				Editor.FadePalette.Init();
				Editor.TerrainPalette.Init();

				foreach (Editor.UndoablePalette fade in Editor.MoreFadePalettes)
				{
					fade?.Init();
				}

				BrushSettingPanel brushPanel = new(owner, this, new(20f, 50f));
				subNodes.Add(brushPanel);

				PreviewSettingsPanel previewPanel = new(owner, this, new(20f, 80f + brushPanel.size.y));
				subNodes.Add(previewPanel);

				PalettePanel._index = 0;
				PalettePanel levelPalettePanel = new(owner, this, new(40f + brushPanel.size.x, 50f), false);
				subNodes.Add(levelPalettePanel);

				if (Editor.TerrainPalette.Texture != null)
				{
					PalettePanel terrainPalettePanel = new(owner, this, new(levelPalettePanel.pos.x + 50f, 80f + levelPalettePanel.size.y), true);
					subNodes.Add(terrainPalettePanel);
				}
			}

			public override void Signal(DevUISignalType type, DevUINode sender, string message)
			{
				SettingsSaveOptions.SaveSignal(this, type, sender, message);

				base.Signal(type, sender, message);
			}

			// The main panels that allow for editing Palette information.
			internal class PalettePanel : Panel
			{
				public static string Name { get; } = "Palette Image";
				public static string ID { get; } = "Palette_Image";
				internal static int _index = 0;

				public static Vector2 PanelSize(bool terrain)
				{
					return Logic.PaletteScaled(terrain) + new Vector2(Margin * 2f, (GenericElementHeight * 2f) + (Margin * 2f));
				}

				// Child elements
				internal PaletteCanvas Canvas { get; }
				public CopySunToRainButton? SunToRain { get; }

				public PalettePanel(DevUI owner, DevUINode parentNode, Vector2 pos, bool terrain) : base(owner, $"{ID}_{_index++}", parentNode, pos, PanelSize(terrain), Name)
				{
					Logic.GetImageSize(terrain, out var imageSize);
					Canvas = new(owner, this, new(Margin, (GenericElementHeight * 2f) + Margin), Logic.PaletteScaled(terrain), terrain);
					subNodes.Add(Canvas);
					if (!terrain)
					{
						SunToRain = new(owner, this);
						subNodes.Add(SunToRain);
					}
				}

				internal class PaletteCanvas : RectangularDevUINode
				{
					public static string ID { get; } = "Palette_Canvas";

					internal Preview PalettePreview { get; set; }
					internal Editor.UndoablePalette? SelectedPalette
					{
						get => _selectedPalette;
						set
						{
							_selectedPalette = value;
							if (_selectedPalette?.Texture != null && PalettePreview != null)
								PalettePreview.Texture = _selectedPalette.Texture;
						}
					}
					private Editor.UndoablePalette? _selectedPalette;

					// Interactive Elements
					public bool WasClicked { get; set; }
					private bool _notUndo;
					private bool _notRedo;
					private bool[,]? _clickedThisFrame;
					public PaletteCanvas(DevUI owner, DevUINode parentNode, Vector2 pos, Vector2 size, bool terrain) : base(owner, ID, parentNode, pos, size)
					{
						Editor.UndoablePalette palette = terrain ? Editor.TerrainPalette : Editor.MainPalette;
						PalettePreview = new(palette?.Texture, Preview.Anchor.BottomLeft, terrain, this);
						if (palette != null)
							palette.PalettePreview = PalettePreview;
						SelectedPalette = palette;
					}

					public override void Refresh()
					{
						base.Refresh();
						PalettePreview.PreviewTexture?.SetPosition(absPos);
						PalettePreview.GrafUpdate(MouseOver);
					}

					public override void Update()
					{
						base.Update();

						PalettePreview.Update(MouseOver);

						// TODO: Fix moving panels over each other causes pixels to be drawn still

						if (MouseOver && !owner.activePage.subNodes.Any((node) => node is Panel panel && panel.dragged))
						{
							List<IntVector2> hoveredPixels = [];
							int brushSize = PalettePreview.HoverPreview.BrushSize;
							for (int x = 0; x < brushSize; x++)
							{
								for (int y = 0; y < brushSize; y++)
								{
									hoveredPixels.Add(PalettePreview.HoveredPixel + new IntVector2(x, y));
								}
							}
							if (Input.GetMouseButton(0))
							{
								WasClicked = true;
								foreach (var i in hoveredPixels)
								{
									if (_clickedThisFrame != null && !_clickedThisFrame[i.x, i.y])
									{
										_clickedThisFrame[i.x, i.y] = true;
										SelectedPalette?.Paint(i.x, i.y);
									}
								}
								SelectedPalette?.Apply(owner.game.cameras[0]);
							}
							else if (!Input.GetMouseButton(0))
							{
								ResetClicked();
								if (WasClicked)
								{
									WasClicked = false;
									SelectedPalette?.AddToStack();
								}
							}

							if (!Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
							{
								IntVector2 hovered = PalettePreview.ExactHoveredPixel;
								SelectedPalette?.PickColor(hovered.x, hovered.y);
							}

							if (Input.mouseScrollDelta.y != 0f)
							{
								if (Input.mouseScrollDelta.y > 0f)
								{
									PalettePreview.HoverPreview.BrushSize++;
								}
								else
								{
									PalettePreview.HoverPreview.BrushSize--;
								}
								PalettePreview.HoverPreview.Resize(PalettePreview.HoverPreview.BrushSize * Logic.PaletteScale);
							}
							if (InputTools.CheckForSingleInput(ref _notUndo, KeyCode.LeftControl, KeyCode.Z))
							{
								SelectedPalette?.Undo(owner.game.cameras[0]);
							}

							if (InputTools.CheckForSingleInput(ref _notRedo, KeyCode.LeftControl, KeyCode.X))
							{
								SelectedPalette?.Redo(owner.game.cameras[0]);
							}
						}
					}

					private void ResetClicked()
					{
						Logic.GetImageSize(PalettePreview.Terrain, out var imageSize);
						_clickedThisFrame = new bool[imageSize.x, imageSize.y];
					}
				}

				internal class CopySunToRainButton : Button
				{
					public CopySunToRainButton(DevUI owner, Panel parentNode) : base(owner, "Sun_To_Rain", parentNode, new(Margin, Margin), 100f, "Copy Sun to Rain")
					{
					}

					public override void Clicked()
					{
						base.Clicked();

						(parentNode as PalettePanel)?.Canvas.SelectedPalette?.CopySunToRain(owner.game.cameras[0]);
					}
				}
			}

			// Holds the brush setting configs
			internal class BrushSettingPanel : Panel, IDevUISignals
			{
				private ColorPicker _colorPicker;

				public static string Name { get; } = "Brush Settings";
				public static string ID { get; } = "Brush_Settings";

				public BrushSettingPanel(DevUI owner, DevUINode parentNode, Vector2 pos) : base(owner, ID, parentNode, pos, new((Margin * 2f) + ColorPicker.ElementSize(true).x, (Margin * 2f) + ColorPicker.ElementSize(true).y), Name)
				{
					_colorPicker = new ColorPickerWithTwoColors(owner, "Color_Picker", this, new(Margin, Margin));
					subNodes.Add(_colorPicker);
				}

				public void Signal(DevUISignalType type, DevUINode sender, string message)
				{
					if (sender == _colorPicker)
					{
						Editor.PaintColor = _colorPicker.ColorValue;
					}
				}

				internal class ColorPickerWithTwoColors : ColorPicker
				{
					private FSprite colorPreviewTwo;
					private bool _switchColor;

					public ColorPickerWithTwoColors(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos) : base(owner, IDstring, parentNode, pos, true)
					{
						// Create a second preview image

						FSprite previewBacking = new("pixel")
						{
							scaleX = 20f,
							scaleY = 15f,
							anchorX = 0,
							anchorY = 0,
							color = Color.grey
						};
						Futile.stage.AddChild(previewBacking);
						fSprites.Add(previewBacking);
						colorPreviewTwo = new FSprite("pixel")
						{
							scaleX = 20f,
							scaleY = 15f,
							anchorX = 0,
							anchorY = 0,
							color = Editor._secondaryPaintColor
						};
						Futile.stage.AddChild(colorPreviewTwo);
						fSprites.Add(colorPreviewTwo);

						fSprites[3].scaleX = 20f;
						fSprites[3].scaleY = 15f;
						colorPreview.scaleX = 20f;
						colorPreview.scaleY = 15f;
						colorPreview.color = Editor._mainPaintColor;

						fSprites[5].MoveBehindOtherNode(fSprites[3]);
						colorPreviewTwo.MoveBehindOtherNode(fSprites[3]);

						ColorValue = Editor.PaintColor;
					}

					public override void Refresh()
					{
						base.Refresh();

						MoveSprite(3, absPos + new Vector2(0f, 5f));
						colorPreview.SetPosition(fSprites[3].GetPosition());
						
						MoveSprite(5, absPos + new Vector2(15f, 0f));
						if (fSprites.Count >= 5)
							colorPreviewTwo?.SetPosition(fSprites[5].GetPosition());
					}

					public override void UpdatePreview()
					{
						if (colorPreviewTwo != null)
						{
							FSprite preview = Editor.CurrentColor == 0 ? colorPreview : colorPreviewTwo;
							preview.color = ColorValue;
							preview.alpha = ColorValue.a;
						}
					}

					public override void Update()
					{
						if (CheckForSingleInput(ref _switchColor, KeyCode.X) && !Input.GetKey(KeyCode.LeftControl))
						{
							Editor.CurrentColor = Editor.CurrentColor == 0 ? 1 : 0;
							ColorValue = Editor.PaintColor;
						}

						base.Update();
					}
				}
			}

			// For changing in-room values for faster previewing
			internal class PreviewSettingsPanel : Panel
			{
				public static string Name { get; } = "Preview Settings";
				public static string ID { get; } = "Preview_Settings";

				private static readonly float __numOfRows = 5f;
				private Vector2 _elemPos;
				private int _line = 0;

				public PreviewSettingsPanel(DevUI owner, DevUINode parentNode, Vector2 pos) : base(owner, ID, parentNode, pos, new Vector2(GenericPanelWidth, (GenericElementHeight * __numOfRows) + (Margin * 2f)) + Padding, Name)
				{
					_elemPos = Padding;
					AddItem(new WaterButton(owner, this, _elemPos));
					AddItem(new WaterSlider(owner, this, _elemPos));
					AddLineBreak();
					AddItem(new NightSlider(owner, this, _elemPos));
					AddItem(new RainSlider(owner, this, _elemPos));
				}

				private void AddItem(DevUINode node)
				{
					subNodes.Add(node);
					_elemPos.y += GenericElementHeight + Margin;
				}

				private void AddLineBreak()
				{
					subNodes.Add(new HorizontalDivider(owner, $"Div_{_line++}", this, _elemPos.y));
					_elemPos.y += Margin * 2f;
				}

				internal static void DestroyTheFUCKINGWater(DevUI owner)
				{
					owner.room.water = false;
					owner.room.waterObject.fWaterLevel = -100f;
					owner.room.waterObject.lastFWaterLevel = -100f;
					owner.game.cameras[0].waterLight?.CleanOut();
					owner.game.cameras[0].waterLight = null;
					owner.room.waterObject.Destroy();
					owner.room.drawableObjects.Remove(owner.room.waterObject);
					owner.room.waterObject = null;
				}

				internal class WaterButton : Button
				{
					public WaterButton(DevUI owner, Panel parentNode, Vector2 pos) : base(owner, "Add_Water", parentNode, pos, 190f, owner.room.waterObject != null ? "Remove Water" : "Add Water") { }

					public override void Clicked()
					{
						if (owner.room.waterObject == null)
						{
							owner.room.defaultWaterLevel = (int)Mathf.Lerp(0f, owner.room.TileHeight, WaterSlider.waterHeight);
							owner.room.AddWater();
						}
						else
						{
							DestroyTheFUCKINGWater(owner);
						}
						Text = owner.room.waterObject != null ? "Remove Water" : "Add Water";
						Refresh();
						base.Clicked();
					}
				}

				internal class WaterSlider : Slider
				{
					private FSprite _waterLinePreview;

					internal static float waterHeight = 0.1f;
					private int _origDefaultWaterLevel = -1;
					private float _lastHeight;

					public WaterSlider(DevUI owner, Panel parentNode, Vector2 pos) : base(owner, "Water_Height", parentNode, pos, "Water Level", false, 60f)
					{
						if (owner.room.waterObject != null)
						{
							waterHeight = owner.room.defaultWaterLevel / owner.room.TileHeight;
							_origDefaultWaterLevel = owner.room.defaultWaterLevel;
						}
						_waterLinePreview = new FSprite("pixel") { scaleX = ScreenSize.x, scaleY = 2f, color = Color.blue, anchorX = 0f };
						Futile.stage.AddChild(_waterLinePreview);
						fSprites.Add(_waterLinePreview);
					}

					public override void NubDragged(float nubPos)
					{
						if (_lastHeight != nubPos)
						{
							_lastHeight = nubPos;

							waterHeight = RoundNub(nubPos);
							parentNode.Refresh();
							Refresh();

							if (owner.room.waterObject != null)
							{
								DestroyTheFUCKINGWater(owner);

								owner.room.defaultWaterLevel = (int)Mathf.Lerp(0f, owner.room.TileHeight, waterHeight);
								owner.room.AddWater();
							}
						}
					}

					private float RoundNub(float nubPos)
					{
						float mul = 20f * (1f / (float)owner.room.TileHeight) * 100f;
						return Mathf.Round(nubPos * mul) / mul;
					}

					public override void Refresh()
					{
						base.Refresh();
						NumberText = ((int)Mathf.Lerp(0f, owner.room.TileHeight, waterHeight)).ToString();
						RefreshNubPos(RoundNub(waterHeight));
						_waterLinePreview.SetPosition(new(0f, (Mathf.Lerp(0f, owner.room.TileHeight, waterHeight) * 20f) - 20f - owner.game.cameras[0].CamPos(0).y));
						_waterLinePreview.MoveToBack();

					}

					public override void ClearSprites()
					{
						base.ClearSprites();

						if (owner.room.waterObject != null)
						{
							owner.room.waterObject.fWaterLevel = -100f;
							owner.room.waterObject.lastFWaterLevel = -100f;
							owner.room.waterObject.Destroy();
							owner.room.waterObject = null;

							if (_origDefaultWaterLevel != -1)
							{
								owner.room.defaultWaterLevel = _origDefaultWaterLevel;
								owner.room.AddWater();
							}
						}
					}
				}

				internal class NightSlider : Slider
				{
					internal static float _nightLerp;
					private float _lastDayNight;

					public NightSlider(DevUI owner, Panel parentNode, Vector2 pos) : base(owner, "Night_Lerp", parentNode, pos, "Night Blend", false, 60f)
					{
						_nightLerp = 0f;
						_lastDayNight = owner.game.cameras[0].effect_dayNight;
						owner.game.cameras[0].effect_dayNight = 1f;
					}

					public override void NubDragged(float nubPos)
					{
						_nightLerp = nubPos;

						int startFade = 1320;
						if (owner.game != null && owner.room != null)
						{
							owner.game.world.rainCycle.dayNightCounter = (int)Mathf.Lerp(0, (float)startFade * 1.92f, _nightLerp);
							if (owner.game.world.rainCycle.dayNightCounter < startFade)
							{
								owner.game.cameras[0]?.ChangeBothPalettes(owner.room.roomSettings.Palette, owner.room.roomSettings.fadePalette?.palette ?? -1, owner.room.roomSettings.fadePalette?.fades[owner.game.cameras[0].currentCameraPosition] ?? 0);
							}
						}

						parentNode.Refresh();
						Refresh();
					}

					public override void Refresh()
					{
						base.Refresh();
						NumberText = $"{Mathf.Round(_nightLerp * 100f)}%";
						RefreshNubPos(_nightLerp);
					}

					public override void ClearSprites()
					{
						base.ClearSprites();
						_nightLerp = 0f;
						owner.game.cameras[0].effect_dayNight = _lastDayNight;
					}
				}

				internal class RainSlider : Slider
				{
					internal static float _rainLerp;
					private float _prevClds;
					public RainSlider(DevUI owner, Panel parentNode, Vector2 pos) : base(owner, "Rain_Lerp", parentNode, pos, "Rain Blend", false, 60f)
					{
						_rainLerp = 0f;
						_prevClds = owner.room.roomSettings.Clouds;
					}

					public override void NubDragged(float nubPos)
					{
						_rainLerp = nubPos;
						owner.room.roomSettings.Clouds = Mathf.Max(_prevClds, _rainLerp);
						parentNode.Refresh();
						Refresh();
					}

					public override void Refresh()
					{
						base.Refresh();
						NumberText = $"{Mathf.Round(_rainLerp * 100f)}%";
						RefreshNubPos(_rainLerp);
					}

					public override void ClearSprites()
					{
						base.ClearSprites();
						owner.room.roomSettings.Clouds = _prevClds;
						_rainLerp = 0f;
					}
				}
			}

			// Small panel to show various tidbits about color information, and more information about keys.
			internal class ColorInformationPanel : Panel
			{
				public static string Name { get; } = "Color Key Information";
				public static string ID { get; } = "Color_Information";

				public ColorInformationPanel(DevUI owner, DevUINode parentNode, Vector2 pos) : base(owner, ID, parentNode, pos, new(GenericPanelWidth, GenericElementHeight), Name)
				{
				}
			}
		}
	}
}
