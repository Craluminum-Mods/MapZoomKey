using System.Reflection;
using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

[assembly: ModInfo("Map Zoom Key")]

namespace MapZoomKey;

public class Core : ModSystem
{
    public const float add = 0.25f;
    public const float substract = -0.25f;

    public const string HarmonyID = "craluminum2413-mapzoomkey";

    public override bool AllowRuntimeReload => true;

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        new Harmony(HarmonyID).PatchAll(Assembly.GetExecutingAssembly());
        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }

    public override void Dispose()
    {
        new Harmony(HarmonyID).UnpatchAll();
        base.Dispose();
    }

    [HarmonyPatch(typeof(GuiElementMap), nameof(GuiElementMap.OnKeyDown))]
    public static class OpenHandbookForEntityPatch
    {
        public static void Postfix(GuiElementMap __instance)
        {
            var api = __instance.Api;

            if (IsKey(api, (int)GlKeys.PageUp) || IsKey(api, (int)GlKeys.Plus))
            {
                __instance.ZoomAdd(
                    zoomDiff: add,
                    px: (float)((api.Input.MouseX - __instance.Bounds.absX) / __instance.Bounds.InnerWidth),
                    pz: (float)((api.Input.MouseY - __instance.Bounds.absY) / __instance.Bounds.InnerHeight));
            }
            else if (IsKey(api, (int)GlKeys.PageDown) || IsKey(api, (int)GlKeys.Minus))
            {
                __instance.ZoomAdd(
                    zoomDiff: substract,
                    px: (float)((api.Input.MouseX - __instance.Bounds.absX) / __instance.Bounds.InnerWidth),
                    pz: (float)((api.Input.MouseY - __instance.Bounds.absY) / __instance.Bounds.InnerHeight));
            }
        }
    }

    private static bool IsKey(ICoreClientAPI api, int key) => api.Input.KeyboardKeyStateRaw[key];
}