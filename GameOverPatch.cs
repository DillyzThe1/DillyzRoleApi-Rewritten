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
        public static bool customWin = false;
        public static string winningRole = "Jester";
        public static List<byte> customWinners = new List<byte>();

        public static void SetAllToWin(String roleToWin, PlayerControl causedBy, bool rpc)
        {
            CustomRole top10Role = CustomRole.getByName(roleToWin);

            if (top10Role.side == CustomRoleSide.Crewmate || top10Role.side == CustomRoleSide.Impostor)
                return;

            customWin = true;
            winningRole = roleToWin;
            customWinners.Clear();
            HarmonyMain.Instance.Log.LogInfo("KILL EM FOR " + roleToWin);
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                string rolename = DillyzUtil.getRoleName(player);
                CustomRoleSide roleside = DillyzUtil.roleSide(player);
                /*if (roleside == top10Role.side && !(top10Role.side == CustomRoleSide.Independent || top10Role.side == CustomRoleSide.LoneWolf))
                    return;*/

                if ((top10Role.side == CustomRoleSide.Independent && rolename != roleToWin) || (top10Role.side == CustomRoleSide.LoneWolf && player != causedBy))
                {
                    HarmonyMain.Instance.Log.LogInfo(player.name + " is now marked as Crewmate!");
                    player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                    //player.Data.Role.Role = AmongUs.GameOptions.RoleTypes.Crewmate;
                    player.Data.Role = new CrewmateRole();

                    if (rpc)
                        player.RpcSetRole(AmongUs.GameOptions.RoleTypes.Crewmate);
                }
                else
                {
                    HarmonyMain.Instance.Log.LogInfo(player.name + " is now marked as Impostor!");
                    player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                    //player.Data.Role.Role = AmongUs.GameOptions.RoleTypes.Impostor;
                    player.Data.Role = new ImpostorRole();

                    if (rpc)
                        player.RpcSetRole(AmongUs.GameOptions.RoleTypes.Impostor);

                    customWinners.Add(player.PlayerId);
                }
            }
            CustomRole.roleNameMap.Clear();
        }

        public static bool didwin = false;
        public static GameOverReason gameOverReason = GameOverReason.HumansByVote;

        public static void Postfix(EndGameManager __instance)
        {
            if (customWin)
            {
                HarmonyMain.Instance.Log.LogInfo("TOP 10 PEOPLE IN MY BASEMENT!!!");
                Color wincolor = CustomRole.getByName(winningRole).roleColor;
                string hexthing = DillyzUtil.colorToHex(wincolor);
                if (customWinners.Contains(PlayerControl.LocalPlayer.PlayerId))
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
            /*else
            {
                if (gameOverReason == GameOverReason.ImpostorByKill || gameOverReason == GameOverReason.ImpostorBySabotage ||
                    gameOverReason == GameOverReason.ImpostorByVote || gameOverReason == GameOverReason.ImpostorDisconnect)
                    hexthing = DillyzUtil.colorToHex(CustomPalette.ImpostorRed);
                else
                    hexthing = DillyzUtil.colorToHex(PlayerControl.LocalPlayer.Data.Role.IsImpostor ? CustomPalette.ImpostorRed : CustomPalette.CrewmateBlue);
            }*/
        }

        // force it to update
        [HarmonyPatch(typeof(TextMeshPro), nameof(TextMeshPro.InternalUpdate))]
        class TMPPROPatch
        {
            public static void Postfix(TextMeshPro __instance)
            {
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

                GameOverPatch.customWin = false;
                GameOverPatch.didwin = false;
                GameOverPatch.winningRole = "";
                GameOverPatch.customWinners.Clear();

                // ahhhh go away
                CustomRole.roleNameMap.Clear();
            }
        }
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        class EndGamePatchIdk {
            public static void Prefix(AmongUsClient __instance, EndGameResult endGameResult) {
                gameOverReason = endGameResult.GameOverReason;

                 if (customWin)
                    return;

                bool impsWon = (gameOverReason == GameOverReason.ImpostorByKill || gameOverReason == GameOverReason.ImpostorBySabotage ||
                    gameOverReason == GameOverReason.ImpostorByVote || gameOverReason == GameOverReason.ImpostorDisconnect);

                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    CustomRoleSide rs = DillyzUtil.roleSide(DillyzUtil.findPlayerControl(player.PlayerId));
                    if (rs == CustomRoleSide.Crewmate || rs == CustomRoleSide.Impostor)
                        continue;

                    if (impsWon)
                    {
                        HarmonyMain.Instance.Log.LogInfo(player.name + " is now marked as Crewmate!");
                        player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                        player.Data.Role = new CrewmateRole();
                    }
                    else
                    {
                        HarmonyMain.Instance.Log.LogInfo(player.name + " is now marked as Impostor!");
                        player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                        player.Data.Role = new ImpostorRole();
                    }
                }
            }
        }
    }
}
