using System;
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DillyzRoleApi_Rewritten
{
    [BepInPlugin(HarmonyMain.MOD_ID, HarmonyMain.MOD_NAME, HarmonyMain.MOD_VERSION)]
    #if RELEASE
    [BepInProcess("Among Us.exe")]
    #endif
    public class HarmonyMain : BasePlugin
    {
        public const string MOD_NAME = "DillyzRoleApi", MOD_VERSION = "2.0.0", MOD_ID = "8ac3dbff-b06b-434b-8783-e0a43e7eeb53";
        public static Harmony harmony = new Harmony(HarmonyMain.MOD_ID);

        public static HarmonyMain Instance;

        public const bool DILLYZ_DEBUG = true;

        public override void Load()
        {
            Instance = this;

            Log.LogInfo($"{HarmonyMain.MOD_NAME} v{HarmonyMain.MOD_VERSION} loaded. Hooray!");
            harmony.PatchAll();

            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, loadscenemode) =>
            {
                Log.LogInfo("this is so sad can we " + scene.name);
                if (scene.name == "MainMenu") // :happyspongebob:
                    ModManager.Instance.ShowModStamp();
            }));

            if (DILLYZ_DEBUG)
            {
                // Change to LoneWolf in the future.
                Log.LogInfo("Adding a Jester!");
                CustomRole.createRole("Jester", "Get voted out to win.", true, false, new Color(90, 50, 200), false,
                                                                        CustomRoleSide.LoneWolf, false, false, true);
                foreach (CustomRole role in CustomRole.allRoles)
                    Log.LogInfo(role.ToString());
            }
         }
    }
}
