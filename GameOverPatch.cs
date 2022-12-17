using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    class GameOverPatch
    {
        public static bool jesterWon = false;

        public static void SetAllToWin(String roleToWin)
        {
            HarmonyMain.Instance.Log.LogInfo("KILL EM FOR " + roleToWin);
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (DillyzUtil.getRoleName(DillyzUtil.findPlayerControl(player.PlayerId)) != roleToWin)
                {
                    HarmonyMain.Instance.Log.LogInfo(player.name + " is now marked as Crewmate!");
                    player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                    player.Data.Role.Role = AmongUs.GameOptions.RoleTypes.Crewmate;
                    player.RpcSetRole(AmongUs.GameOptions.RoleTypes.Crewmate);
                }
                else
                {
                    HarmonyMain.Instance.Log.LogInfo(player.name + " is now marked as Impostor!");
                    player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                    player.Data.Role.Role = AmongUs.GameOptions.RoleTypes.Impostor;
                    player.RpcSetRole(AmongUs.GameOptions.RoleTypes.Impostor);
                }
            }
        }

        public static string hexthing = "";
        public static bool didwin = false;
        public static Color wincolor = new Color(255,255,255);

        public static void Postfix(EndGameManager __instance)
        {
            if (jesterWon)
            {
                HarmonyMain.Instance.Log.LogInfo("TOP 10 PEOPLE IN MY BASEMENT!!!");
                HarmonyMain.Instance.Log.LogInfo(DillyzUtil.getRoleName(PlayerControl.LocalPlayer) + " playuer");
                wincolor = CustomRole.getByName("Jester").roleColor;
                hexthing = DillyzUtil.colorToHex(wincolor);
                didwin = DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "Jester";
                if (didwin)
                    __instance.WinText.text = $"<{hexthing}>Victory</color>";
                else
                    __instance.WinText.text = $"<{hexthing}>Defeat</color>";
                __instance.WinText.material.color = wincolor;
                __instance.BackgroundBar.material.color = wincolor;

                /*List<PoolablePlayer> the = __instance.GetComponentsInChildren<PoolablePlayer>().ToArray().ToList();
                for (int i = 0; i < the.Count; i++)
                    GameObject.Destroy(the[i]);

                List<PlayerControl> jestersHaha = PlayerControl.AllPlayerControls.ToArray().ToList();
                jestersHaha.RemoveAll(x => DillyzUtil.getRoleName(x) != "Jester");
                foreach (PlayerControl player in jestersHaha)
                {
                    PoolablePlayer poolableInstance = new PoolablePlayer();
                    poolableInstance.SetHat(player.CurrentOutfit.HatId, player.cosmetics.hat.matProperties.ColorId);
                    poolableInstance.SetBodyColor(player.CurrentOutfit.ColorId);
                    poolableInstance.SetName(player.name);
                    poolableInstance.SetVisor(player.CurrentOutfit.VisorId, player.cosmetics.visor.matProperties.ColorId);
                    poolableInstance.cosmetics.SetSkin(player.CurrentOutfit.SkinId, player.cosmetics.skin.matProperties.ColorId);
                    poolableInstance.cosmetics.InstantiatePetCopy(player.cosmetics.currentPet, player.CurrentOutfit.ColorId);

                    poolableInstance.transform.Find("Names").gameObject.active = false;
                }*/
            }
        }

        // force it to update
        [HarmonyPatch(typeof(TextMeshPro), nameof(TextMeshPro.InternalUpdate))]
        class TMPPROPatch
        {
            public static void Postfix(TextMeshPro __instance)
            {
                if (jesterWon)
                {
                    if (__instance.name == "WinText")
                    {
                        if (didwin)
                            __instance.text = $"<{hexthing}>Victory</color>";
                        else
                            __instance.text = $"<{hexthing}>Defeat</color>";
                    }
                    __instance.material.color = wincolor;
                }


                if (__instance.name == "YouAreText")
                    __instance.text = $"<{IntroCutscenePatch.colorHex}>Your role is</color>";
            }
        }
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGame))]
        class WadawdawawawfwafPatch
        {
            public static void Postfix(EndGameManager __instance)
            {
                HarmonyMain.Instance.Log.LogInfo("GAME STARTS NOW?!?!");
                // waffle_iron.jpeg

                GameOverPatch.jesterWon = false;
                GameOverPatch.didwin = false;
                GameOverPatch.hexthing = "#000000";

                // ahhhh go away
                try
                {
                    foreach (byte pId in CustomRole.roleNameMap.Keys)
                        CustomRole.roleNameMap[pId] = "";
                } catch (Exception e) { 
                    HarmonyMain.Instance.Log.LogError("GOOFY GOOFY GOOBER GOOBER YEAHHHH " + e); 
                }
            }
        }
    }
}
