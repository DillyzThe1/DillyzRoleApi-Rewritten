using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace DillzyRoleApi_Rewritten
{
    [BepInPlugin(HarmonyMain.MOD_ID, HarmonyMain.MOD_NAME, HarmonyMain.MOD_VERSION)]
    [BepInProcess("Among Us.exe")]
    public class HarmonyMain : BasePlugin
    {
        public const string MOD_NAME = "DillyzRoleApi Rewritten", MOD_VERSION = "0.1.0", MOD_ID = "8ac3dbff-b06b-434b-8783-e0a43e7eeb53";
        public override void Load()
        {
            Log.LogInfo($"{HarmonyMain.MOD_NAME} v{HarmonyMain.MOD_VERSION} loaded.");

            #region ---------- Enable Harmony Patching ----------
            var harmony = new Harmony(HarmonyMain.MOD_ID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            #endregion
        }
    }
}
