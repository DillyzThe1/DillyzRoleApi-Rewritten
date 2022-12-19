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
    [BepInProcess("Among Us.exe")]
    [BepInProcess("Among Us2.exe")]
    [BepInProcess("Among Us3.exe")]
    [BepInProcess("Among Us4.exe")]
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
                Log.LogInfo("Adding a Jester!");
                CustomRole.createRole("Jester", "Get voted out to win.", true, false, new Color(90, 50, 200), false,
                                                                        CustomRoleSide.LoneWolf, VentPrivilege.Impostor, false, true);
                CustomRole.getByName("Jester").a_or_an = "a";

                Log.LogInfo("Adding a Sheriff!");
                CustomRole.createRole("Sheriff", "Kill the impostor or suicide.", true, true, new Color(255, 185, 30), false,
                                                                        CustomRoleSide.Crewmate, VentPrivilege.None, true, true);
                CustomRole.getByName("Sheriff").a_or_an = "a";

                Log.LogInfo("Adding a funny button!");
                CustomButton.addButton("Fred", "DillyzRoleApi-Rewritten.Assets.uncle_fred.png", 20, true, new string[] { "Scientist" }, new string[] {}, 
                delegate (CustomActionButton button, bool success) {
                    if (!success)
                        return;

                    Log.LogInfo(button.curTarget.name + " was targetted by fred!");

                    DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, button.curTarget);
                });

                foreach (CustomRole role in CustomRole.allRoles)
                    Log.LogInfo(role.ToString());
            }
         }
    }
}
