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
            foreach (CustomButton button in CustomButton.AllCustomButtons)
                button.GameInstance = null;

            CustomRole top10Role = CustomRole.getByName(roleToWin);

            if (top10Role.side == CustomRoleSide.Crewmate || top10Role.side == CustomRoleSide.Impostor)
                return;

            customWin = true;
            winningRole = roleToWin;
            customWinners.Clear();
            DillyzRoleApiMain.Instance.Log.LogInfo("KILL EM FOR " + roleToWin);
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                string rolename = DillyzUtil.getRoleName(player);
                CustomRoleSide roleside = DillyzUtil.roleSide(player);

                if ((top10Role.side == CustomRoleSide.Independent && rolename != roleToWin) || (top10Role.side == CustomRoleSide.LoneWolf && player != causedBy))
                {
                    DillyzRoleApiMain.Instance.Log.LogInfo(player.name + " is now marked as Crewmate!");
                    player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                    player.Data.Role = new CrewmateRole();

                    if (rpc)
                        player.RpcSetRole(AmongUs.GameOptions.RoleTypes.Crewmate);
                }
                else
                {
                    DillyzRoleApiMain.Instance.Log.LogInfo(player.name + " is now marked as Impostor!");
                    player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
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
                DillyzRoleApiMain.Instance.Log.LogInfo("TOP 10 PEOPLE IN MY BASEMENT!!!");
                Color32 wincolor = CustomRole.getByName(winningRole).roleColor;
                string hexthing = DillyzUtil.colorToHex(wincolor);
                if (customWinners.Contains(PlayerControl.LocalPlayer.PlayerId))
                    __instance.WinText.text = $"<{hexthing}>Victory</color>";
                else
                    __instance.WinText.text = $"<{hexthing}>Defeat</color>";
                __instance.WinText.material.color = wincolor;
                __instance.BackgroundBar.material.color = wincolor;
            }
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
                DillyzRoleApiMain.Instance.Log.LogInfo("GAME STARTS NOW?!?!");
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

                List<PlayerControl> playersLol = PlayerControl.AllPlayerControls.ToArray().ToList();
                DillyzRoleApiMain.Instance.Log.LogInfo("got pipe bombed " + playersLol.Count);

                if (customWin)
                {
                    foreach (PlayerControl player in playersLol)
                    {
                        if (customWinners.Contains(player.PlayerId))
                        {
                            DillyzRoleApiMain.Instance.Log.LogInfo(player.name + " is now marked as Impostor!");
                            player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                            player.Data.Role = new ImpostorRole();
                        }
                        else
                        {
                            DillyzRoleApiMain.Instance.Log.LogInfo(player.name + " is now marked as Crewmate!");
                            player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                            player.Data.Role = new CrewmateRole();
                        }
                    }

                    CustomRole.roleNameMap.Clear();
                    return;
                }

                bool impsWon = (gameOverReason == GameOverReason.ImpostorByKill || gameOverReason == GameOverReason.ImpostorBySabotage ||
                    gameOverReason == GameOverReason.ImpostorByVote || gameOverReason == GameOverReason.ImpostorDisconnect);

                foreach (PlayerControl player in playersLol)
                {
                    CustomRoleSide rs = DillyzUtil.roleSide(player);
                    if (rs == CustomRoleSide.Crewmate)
                    {
                        DillyzRoleApiMain.Instance.Log.LogInfo(player.name + " is now marked as Crewmate!");
                        player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                        player.Data.Role = new CrewmateRole();
                        continue;
                    }
                    if (rs == CustomRoleSide.Impostor)
                    {
                        DillyzRoleApiMain.Instance.Log.LogInfo(player.name + " is now marked as Impostor!");
                        player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                        player.Data.Role = new ImpostorRole();
                        continue;
                    }

                    if (impsWon)
                    {
                        DillyzRoleApiMain.Instance.Log.LogInfo(player.name + " is now marked as Crewmate!");
                        player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                        player.Data.Role = new CrewmateRole();
                    }
                    else
                    {
                        DillyzRoleApiMain.Instance.Log.LogInfo(player.name + " is now marked as Impostor!");
                        player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                        player.Data.Role = new ImpostorRole();
                    }
                }

                CustomRole.roleNameMap.Clear();
            }
        }


    }
}
