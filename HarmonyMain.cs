using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
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

        // https://docs.bepinex.dev/v5.4.21/articles/dev_guide/plugin_tutorial/4_configuration.html
        public ConfigEntry<bool> enableDebugJester;
        public ConfigEntry<bool> enableDebugSheriff;

        public override void Load()
        {
            Instance = this;

            enableDebugJester = Config.Bind("Debug", "Debug Jester", false,
                "Enables the Debug Jester built into the API.\nThis functions normally, but you may want to use the official package here:\n" +
                "https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/Packages.md");
            enableDebugSheriff = Config.Bind("Debug", "Debug Sheriff", false,
                "Enables the Debug Sheriff built into the API.\nThis functions normally, but you may want to use the official package here:\n" +
                "https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/Packages.md");

            Log.LogInfo($"{HarmonyMain.MOD_NAME} v{HarmonyMain.MOD_VERSION} loaded. Hooray!");
            harmony.PatchAll();

            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, loadscenemode) =>
            {
                Log.LogInfo("this is so sad can we " + scene.name);
                if (scene.name == "MainMenu") // :happyspongebob:
                    ModManager.Instance.ShowModStamp();
            }));

            if (enableDebugJester.Value)
            {
                Log.LogInfo("Adding a Jester!");
                DillyzUtil.createRole("Jester", "Get voted out to win.", true, false, new Color32(90, 50, 200, 255), false,
                                                                        CustomRoleSide.LoneWolf, VentPrivilege.None, false, true);
                CustomRole.getByName("Jester").a_or_an = "a";
            }

            if (enableDebugSheriff.Value)
            {
                Log.LogInfo("Adding a Sheriff!");
                DillyzUtil.createRole("Sheriff", "Kill the impostor or suicide.", true, true, new Color32(255, 185, 30, 255), false,
                                                                        CustomRoleSide.Crewmate, VentPrivilege.None, false, true);
                CustomRole.getByName("Sheriff").a_or_an = "a";

                Log.LogInfo("Adding a sherrif button!");
                DillyzUtil.addButton(Assembly.GetExecutingAssembly(), "Sheriff Kill Button", "DillyzRoleApi_Rewritten.Assets.sheriff_kill.png", -1f, true,
                new string[] { "Sheriff" }, new string[] { }, delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    Log.LogInfo(button.killButton.currentTarget.name + " was targetted by fred!");

                    DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, button.killButton.currentTarget);

                    if (DillyzUtil.roleSide(button.killButton.currentTarget) == CustomRoleSide.Crewmate)
                        DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);

                });

                CustomButton.getButtonByName("Sheriff Kill Button").buttonText = "Kill";
                CustomButton.getButtonByName("Sheriff Kill Button").textOutlineColor = new Color32(255, 185, 30, 255);
            }

            //foreach (CustomRole role in CustomRole.allRoles)
            //    Log.LogInfo(role.ToString());

            IL2CPPChainloaderPatch.Reg(Assembly.GetExecutingAssembly());

            string[] layers = Enumerable.Range(0, 32).Select(index => LayerMask.LayerToName(index)).Where(l => !string.IsNullOrEmpty(l)).ToArray();
            string outputLayerStr = "Available Layers: ";
            for (int i = 0; i < layers.Length; i++)
                outputLayerStr += layers[i] + (i == layers.Length - 1 ? "." : ", ");
            Log.LogInfo(outputLayerStr);
        }
    }
}
