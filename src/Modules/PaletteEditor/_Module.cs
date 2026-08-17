using DevInterface;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RegionKit.Modules.DevUIMisc;

namespace RegionKit.Modules.PaletteEditor
{

	[RegionKitModule(nameof(Enable), nameof(Disable), moduleName: "PaletteEditor")]
	internal class _Module
	{
		internal static Hook? _RoomCameraDarkPaletteHook { get; private set; }

		private static void Enable()
		{
			IL.AboveCloudsView.Update += IgnoreCycleTimer;
			IL.RoomCamera.UpdateDayNightPalette += IgnoreCycleTimer;

			On.WaterLight.DrawUpdate += WaterLight_DrawUpdate;
			_RoomCameraDarkPaletteHook = new Hook(typeof(RoomCamera).GetProperty(nameof(RoomCamera.DarkPalette), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetGetMethod(true), RoomCamera_DarkPalette);
			On.RainWorldGame.AllowRainCounterToTick += RainWorldGame_AllowRainCounterToTick;

			On.Menu.Remix.MixedUI.OpColorPicker.DisplayDescription += OpColorPicker_DisplayDescription;
			On.RoomCamera.ApplyEffectColorsToPaletteTexture += RoomCamera_ApplyEffectColorsToPaletteTexture;
			IL.RoomCamera.LoadPalette += RoomCamera_LoadPalette;
			On.RoomCamera.ApplyPalette += RoomCamera_ApplyPalette;

			On.DevInterface.DevUI.ctor += DevUI_ctor;
			On.DevInterface.Page.SwitchPageButtonPos += Page_SwitchPageButtonPos;
			On.DevInterface.DevUI.SwitchPage += DevUI_SwitchPage;
		}

		private static void Disable()
		{
			IL.AboveCloudsView.Update -= IgnoreCycleTimer;
			IL.RoomCamera.UpdateDayNightPalette -= IgnoreCycleTimer;

			On.WaterLight.DrawUpdate -= WaterLight_DrawUpdate;
			_RoomCameraDarkPaletteHook?.Undo();
			On.RainWorldGame.AllowRainCounterToTick -= RainWorldGame_AllowRainCounterToTick;

			On.Menu.Remix.MixedUI.OpColorPicker.DisplayDescription -= OpColorPicker_DisplayDescription;
			On.RoomCamera.ApplyEffectColorsToPaletteTexture -= RoomCamera_ApplyEffectColorsToPaletteTexture;
			IL.RoomCamera.LoadPalette -= RoomCamera_LoadPalette;
			On.RoomCamera.ApplyPalette -= RoomCamera_ApplyPalette;

			On.DevInterface.DevUI.ctor -= DevUI_ctor;
			On.DevInterface.Page.SwitchPageButtonPos -= Page_SwitchPageButtonPos;
			On.DevInterface.DevUI.SwitchPage -= DevUI_SwitchPage;
		}

		private static void IgnoreCycleTimer(ILContext il)
		{
			try
			{
				ILCursor c = new(il);

				c.GotoNext(x => x.MatchLdfld(typeof(RainCycle).GetField(nameof(RainCycle.cycleLength), BF_ALL_CONTEXTS_INSTANCE)));
				c.GotoNext(x => x.MatchBge(out _));

				static int RainCycleLimit(int origLength)
				{
					if (rainWorld.processManager.currentMainLoop is RainWorldGame game && game.devUI?.activePage is DevPaletteEditor.NewPalettePage)
					{
						return 0;
					}
					return origLength;
				}
				// Modify any instance of the rain timer being checked so we can do fun stuff!! Yuppee!!
				c.EmitDelegate(RainCycleLimit);

				LogInfo(il.ToString());
			}
			catch (Exception ex)
			{
				LogError(ex);
			}
		}

		private static void WaterLight_DrawUpdate(On.WaterLight.orig_DrawUpdate orig, WaterLight self, Vector2 camPos)
		{
			// Attempt to fix a missing null check in Vanilla
			if (self.waterObject != null)
			{
				orig(self, camPos);
			}
		}

		private static float RoomCamera_DarkPalette(Func<RoomCamera, float> orig, RoomCamera self)
		{
			if (self.game?.devToolsActive ?? false && self.game?.devUI.activePage is DevPaletteEditor.NewPalettePage)
			{
				return DevPaletteEditor.NewPalettePage.PreviewSettingsPanel.RainSlider._rainLerp;
			}
			return orig(self);
		}

		private static bool RainWorldGame_AllowRainCounterToTick(On.RainWorldGame.orig_AllowRainCounterToTick orig, RainWorldGame self)
		{
			return orig(self) && (!self.devToolsActive || self.devUI?.activePage is not DevPaletteEditor.NewPalettePage);
		}

		private static string OpColorPicker_DisplayDescription(On.Menu.Remix.MixedUI.OpColorPicker.orig_DisplayDescription orig, Menu.Remix.MixedUI.OpColorPicker self)
		{
			if (Custom.rainWorld.processManager.currentMainLoop is RainWorldGame) return "";
			return orig(self);
		}

		private static void RoomCamera_ApplyEffectColorsToPaletteTexture(On.RoomCamera.orig_ApplyEffectColorsToPaletteTexture orig, RoomCamera self, ref Texture2D texture, int color1, int color2)
		{
			orig(self, ref texture, color1, color2);

			texture.Apply();
		}

		private static void RoomCamera_LoadPalette(ILContext il)
		{
			ILCursor c = new(il);

			static string PrioritizeSavedPalettes(string text, int pal)
			{
				if (ModOptions.LoadSavedPalettes.Value)
				{
					//string newPath = Path.Combine(DevPaletteEditor.NewPalettePage.SmallElements.SaveButton.SavePath, $"palette{pal}.png");
					//if (File.Exists(newPath))
					//{
					//	return newPath;
					//}
				}
				return text;
			}

			c.GotoNext(x => x.MatchStloc(0));
			c.Emit(OpCodes.Ldarg_1);
			c.EmitDelegate(PrioritizeSavedPalettes);

			static void ReassignPalettes(RoomCamera self, int pal, ref Texture2D texture)
			{
				if (texture == self.fadeTexA)
				{
					Editor.MainPalette.Texture = texture;
				}
				if (texture == self.fadeTexB)
				{
					Editor.FadePalette.Texture = texture;
				}
			}

			c.GotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchCallOrCallvirt(typeof(RoomCamera).GetProperty(nameof(RoomCamera.room)).GetGetMethod())
				);

			c.MoveAfterLabels();
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_1);
			c.Emit(OpCodes.Ldarg_2);
			c.EmitDelegate(ReassignPalettes);
		}

		private static void RoomCamera_ApplyPalette(On.RoomCamera.orig_ApplyPalette orig, RoomCamera self)
		{
			if (self.room != null && self.room.terrain != null && (self.room.roomSettings.TerrainPalette == null || self.room.roomSettings.TerrainPalette == "NO PALETTE"))
			{
				self.room.roomSettings.TerrainPalette = Logic.DefaultTerrainPalette;
			}

			orig(self);

			if (self.room != null && self.terrainPalette != null && self.room.roomSettings.TerrainPalette != null)
			{
				Texture2D mainTex = self.terrainPalette.mainPal.LoadTex(self.terrainPalette.mainPal.name);
				Editor.TerrainPalette.Texture = mainTex;
				Logic.TerrainImageSize = new(mainTex.width, mainTex.height);
				Logic.TerrainPaletteKeys = Logic.Key.InitTerrainPalette(mainTex.width, mainTex.height);
			}
		}

		private static void DevUI_ctor(On.DevInterface.DevUI.orig_ctor orig, DevUI self, RainWorldGame game)
		{
			orig(self, game);

			if (!self.pages.Contains(DevPaletteEditor.NewPalettePage.Name))
				self.pages = [.. self.pages, DevPaletteEditor.NewPalettePage.Name];
		}

		private static Vector2 Page_SwitchPageButtonPos(On.DevInterface.Page.orig_SwitchPageButtonPos orig, Page self, int i, string name)
		{
			if (name == DevPaletteEditor.NewPalettePage.Name)
			{
				return new Vector2(100f, DevUIUtils.__switchPageButtonY);
			}

			Vector2 result = orig(self, i, name);
			// Change default Relationship button y location to match the other buttons
			if (name == "Relationships")
			{
				result.y = DevUIUtils.__switchPageButtonY;
			}
			return result;
		}

		private static void DevUI_SwitchPage(On.DevInterface.DevUI.orig_SwitchPage orig, DevUI self, int newPage)
		{

			if (!self.pages.Contains(DevPaletteEditor.NewPalettePage.Name))
				self.pages = [.. self.pages, DevPaletteEditor.NewPalettePage.Name];

			if (newPage == self.pages.IndexOf(DevPaletteEditor.NewPalettePage.Name))
			{
				self.ClearSprites();
				self.activePage = new DevPaletteEditor.NewPalettePage(self);
				return;
			}

			orig(self, newPage);
		}
	}
}
