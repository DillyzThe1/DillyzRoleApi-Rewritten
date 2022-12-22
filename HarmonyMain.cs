using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DillyzRoleApi_Rewritten.Il2CppItemAttribute;

namespace DillyzRoleApi_Rewritten
{
    [BepInPlugin(HarmonyMain.MOD_ID, HarmonyMain.MOD_NAME, HarmonyMain.MOD_VERSION)]
    [BepInProcess("Among Us.exe")]
    [BepInProcess("Among Us2.exe")]
    [BepInProcess("Among Us3.exe")]
    [BepInProcess("Among Us4.exe")]
    [BepInProcess("Among Us5.exe")]
    [BepInProcess("Among Us6.exe")]
    [BepInProcess("Among Us7.exe")]
    [BepInProcess("Among Us8.exe")]
    [BepInProcess("Among Us9.exe")]
    [BepInProcess("Among Us10.exe")]
    [BepInProcess("Among Us11.exe")]
    [BepInProcess("Among Us12.exe")]
    [BepInProcess("Among Us13.exe")]
    [BepInProcess("Among Us14.exe")]
    [BepInProcess("Among Us15.exe")]
    public class HarmonyMain : BasePlugin
    {
        public const string MOD_NAME = "DillyzRoleApi", MOD_VERSION = "2.0.0", MOD_ID = "com.github.dillyzthe1.dillyzroleapi";
        public static Harmony harmony = new Harmony(HarmonyMain.MOD_ID);

        public static HarmonyMain Instance;

        public const bool DILLYZ_DEBUG = false;

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
                DillyzUtil.createRole("Jester", "Get voted out to win.", true, false, new Color(90, 50, 200), false,
                                                                        CustomRoleSide.LoneWolf, VentPrivilege.None, false, true);
                CustomRole.getByName("Jester").a_or_an = "a";

                Log.LogInfo("Adding a Sheriff!");
                DillyzUtil.createRole("Sheriff", "Kill the impostor or suicide.", true, true, new Color(255, 185, 30), false,
                                                                        CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
                CustomRole.getByName("Sheriff").a_or_an = "a";

                Log.LogInfo("Adding a funny button!");
                DillyzUtil.addButton("Fred", "DillyzRoleApi_Rewritten.Assets.uncle_fred.png", 20, true, new string[] { "Sheriff" }, new string[] {}, 
                delegate (KillButtonCustomData button, bool success) {
                    if (!success)
                        return;

                    Log.LogInfo(button.killButton.currentTarget.name + " was targetted by fred!");

                    DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, button.killButton.currentTarget);
                });

                foreach (CustomRole role in CustomRole.allRoles)
                    Log.LogInfo(role.ToString());
            }

            IL2CPPChainloaderPatch.Reg(Assembly.GetExecutingAssembly());
         }
    }
}
