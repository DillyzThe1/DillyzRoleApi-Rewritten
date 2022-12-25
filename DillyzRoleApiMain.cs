using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DillyzRoleApi_Rewritten.Il2CppItemAttribute;

namespace DillyzRoleApi_Rewritten
{
    [BepInPlugin(DillyzRoleApiMain.MOD_ID, DillyzRoleApiMain.MOD_NAME, DillyzRoleApiMain.MOD_VERSION)]
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
    public class DillyzRoleApiMain : BasePlugin
    {
        public const string MOD_NAME = "DillyzRoleApi", MOD_VERSION = "2.0.0", MOD_ID = "com.github.dillyzthe1.dillyzroleapi";
        public static Harmony harmony = new Harmony(DillyzRoleApiMain.MOD_ID);

        public static string BEPINEX_CONFIG_FOLDER => BepInEx.Paths.BepInExConfigPath.Replace("BepInEx.cfg","");

        public static DillyzRoleApiMain Instance;

        // https://docs.bepinex.dev/v5.4.21/articles/dev_guide/plugin_tutorial/4_configuration.html
        public ConfigEntry<bool> enableDebugHitman;

        public override void Load()
        {
            Instance = this;

            enableDebugHitman = Config.Bind("Debug", "Debug Hitman", false,
                "Enables the Debug Hitman built into the API.\nThis functions normally, but you may want to use the official package here:\n" +
                "https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/Packages.md");

            Log.LogInfo($"{DillyzRoleApiMain.MOD_NAME} v{DillyzRoleApiMain.MOD_VERSION} loaded. Hooray!");
            harmony.PatchAll();

            SceneManager.add_sceneLoaded((System.Action<Scene, LoadSceneMode>)((scene, loadscenemode) =>
            {
                Log.LogInfo("this is so sad can we " + scene.name);
                if (scene.name == "MainMenu") // :happyspongebob:
                    ModManager.Instance.ShowModStamp();
            }));

            LobbyConfigManager.Load();

            if (enableDebugHitman.Value) {
                Log.LogInfo("Adding a Hitman!");
                DillyzUtil.createRole("Hitman", "You must work alone to succeed.", true, false, new Color32(75,65,85,255), false, 
                                                                     CustomRoleSide.LoneWolf,  VentPrivilege.Impostor, false, true);
                CustomRole hitmanRole = CustomRole.getByName("Hitman");
                hitmanRole.a_or_an = "a";
                hitmanRole.SetSprite(Assembly.GetExecutingAssembly(), "DillyzRoleApi_Rewritten.Assets.hitman_kill.png");
                hitmanRole.returnWinConditionState = delegate () {
                    List<PlayerControl> goobers = hitmanRole.AllPlayersWithRole;
                    goobers.RemoveAll(x => x.Data.IsDead);

                    if (goobers.Count <= 0)
                        return WinConditionState.None;

                    List<PlayerControl> goofy = PlayerControl.AllPlayerControls.ToArray().ToList();
                    goofy.RemoveAll(x => x.Data.IsDead);

                    if (goobers.Count == 1 && goofy.Count == 1)
                        hitmanRole.WinGame(goofy[0]);

                    if (goofy.Count == 2) {
                        string rolename_1 = DillyzUtil.getRoleName(goofy[0]);
                        string rolename_2 = DillyzUtil.getRoleName(goofy[1]);

                        CustomRoleSide roleside_1 = DillyzUtil.roleSide(goofy[0]);
                        CustomRoleSide roleside_2 = DillyzUtil.roleSide(goofy[1]);

                        if (rolename_1 != "Hitman" && roleside_1 != CustomRoleSide.Impostor && rolename_2 != "Hitman" && roleside_2 != CustomRoleSide.Impostor)
                            return WinConditionState.None;

                        if (rolename_1 != "Hitman" && rolename_2 != "Hitman")
                            return WinConditionState.None;

                        if (rolename_2 != "Hitman" && roleside_2 != CustomRoleSide.Impostor && rolename_1 == "Hitman")
                            hitmanRole.WinGame(goofy[0]);

                        if (rolename_1 != "Hitman" && roleside_1 != CustomRoleSide.Impostor && rolename_2 == "Hitman")
                            hitmanRole.WinGame(goofy[1]);
                    }

                     return WinConditionState.Hold; 
                };

                DillyzUtil.addButton(Assembly.GetExecutingAssembly(), "Hitman Kill Button", "DillyzRoleApi_Rewritten.Assets.hitman_kill.png", -0.75f, true,
                new string[] { "Hitman" }, new string[] { }, delegate (KillButtonCustomData button, bool success)
                {
                    if (!success)
                        return;

                    Log.LogInfo(button.killButton.currentTarget.name + " was targetted by " + PlayerControl.LocalPlayer.name + "!");

                    DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, button.killButton.currentTarget);
                });
                CustomButton.getButtonByName("Hitman Kill Button").buttonText = "Kill";
                CustomButton.getButtonByName("Hitman Kill Button").textOutlineColor = new Color32(75, 65, 85, 255);
            }

            //foreach (CustomRole role in CustomRole.allRoles)
            //    Log.LogInfo(role.ToString());

            IL2CPPChainloaderPatch.Reg(Assembly.GetExecutingAssembly());

            string[] layers = Enumerable.Range(0, 32).Select(index => LayerMask.LayerToName(index)).Where(l => !string.IsNullOrEmpty(l)).ToArray();
            string outputLayerStr = "Available Layers: ";
            for (int i = 0; i < layers.Length; i++)
                outputLayerStr += layers[i] + (i == layers.Length - 1 ? "." : ", ");
            Log.LogInfo(outputLayerStr);

            Log.LogInfo(BEPINEX_CONFIG_FOLDER);

            /*var regions = ServerManager.DefaultRegions.ToList();
            string iptouse = "localhost";
            /*if (Uri.CheckHostName(iptouse).ToString() == "Dns")
                foreach (IPAddress address in Dns.GetHostAddresses(iptouse))
                    if (address.AddressFamily == Il2CppSystem.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        iptouse = address.ToString();
                        break;
                    }*

            IRegionInfo reg = new IRegionInfo() { Name = "" };

            regions.Insert(0, reg);*/ 
        }

    }
}
