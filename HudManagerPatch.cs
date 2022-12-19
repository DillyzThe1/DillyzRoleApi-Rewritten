using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TMPro;
using UnityEngine;
using AmongUs.GameOptions;
using InnerNet;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    class HudManagerStartPatch
    {
        public static void Postfix(HudManager __instance) {
            HudManagerPatch.lastKillThingForCustoms = DateTime.UtcNow;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerPatch
    {
        public static DateTime lastKillThingForCustoms = DateTime.UtcNow;
        public static void Postfix(HudManager __instance)
        {
            if (/*AmongUsClient.Instance == null || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started ||*/ PlayerControl.LocalPlayer == null ||
                PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.Role == null)
            {
                //foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                //    HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
                return;
            }

            // colorization
            string rnnnmn = DillyzUtil.getRoleName(PlayerControl.LocalPlayer);
            CustomRole localRole = (rnnnmn != null && rnnnmn != "") ? CustomRole.getByName(rnnnmn) : null;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                bool youre = player.PlayerId == PlayerControl.LocalPlayer.PlayerId;

                string rnnnn = DillyzUtil.getRoleName(player);
                CustomRole theRole = (rnnnn != null && rnnnn != "") ? CustomRole.getByName(rnnnn) : null;

                // if the role is not there or doesn't change color, leave it alone
                if (theRole == null || !theRole.nameColorChanges)
                {
                    if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && player.Data.Role.IsImpostor)
                        HudManagerPatch.displayColor(__instance, player, (player.Data.RoleType == RoleTypes.Shapeshifter) ?
                                                                        CustomPalette.ShapeShifterCrimson : CustomPalette.ImpostorRed);
                    else if (youre)
                        HudManagerPatch.displayColor(__instance, player, DillyzUtil.roleColor(player, true));
                    else
                        HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
                    continue;
                }

                 if (theRole.nameColorPublic || youre || // If the name color was public. || if it's you
                    // If it's private but you get it.
                    (theRole.teamCanSeeYou && localRole != null && theRole.name == localRole.name && theRole.side != CustomRoleSide.LoneWolf)) 
                    HudManagerPatch.displayColor(__instance, player, theRole.roleColor);
                else
                    HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
            }

            if (AmongUsClient.Instance == null || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
                return;

            // task list
            bool udiededed = PlayerControl.LocalPlayer.Data.IsDead;
            if (!udiededed || DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "GuardianAngel")
            {
                string intendedString = DillyzUtil.roleText(PlayerControl.LocalPlayer);
                if (__instance.TaskPanel.taskText.text.Length > 0 && !__instance.TaskPanel.taskText.text.Contains(intendedString))
                {
                    if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Scientist)
                        __instance.TaskPanel.taskText.text = 
                            __instance.TaskPanel.taskText.text.Substring(0, __instance.TaskPanel.taskText.text.IndexOf("Scientist Hint") - 1);
                    else if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Engineer)
                        __instance.TaskPanel.taskText.text =
                            __instance.TaskPanel.taskText.text.Substring(0, __instance.TaskPanel.taskText.text.IndexOf("Engineer Hint") - 1);
                    else if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.GuardianAngel)
                        __instance.TaskPanel.taskText.text =
                            __instance.TaskPanel.taskText.text.Substring(0, __instance.TaskPanel.taskText.text.IndexOf("Guardian Angel") - 1);

                    __instance.TaskPanel.taskText.text += intendedString;
                }
            }

            // DOING THIS FOR THE RETURN THING BC I LIKE RETURNS
            displayActionButton(__instance, localRole, udiededed);
        }

        public static void displayActionButton(HudManager __instance, CustomRole localRole, bool udiededed) {
            if (localRole != null)
            {
                if (MeetingHud.Instance != null)
                {
                    __instance.ImpostorVentButton.gameObject.active = false;
                    __instance.KillButton.gameObject.SetActive(false);
                    return;
                }
                //GameObject buttonIWant = __instance.Find("Buttons").transform.Find("BottomRight").gameObject;
                //buttonIWant.transform.Find("").gameObject.active = localRole.ventPrivilege == VentPrivilege.Impostor;
                __instance.ImpostorVentButton.gameObject.active = (localRole.ventPrivilege == VentPrivilege.Impostor) && !udiededed;

                __instance.KillButton.gameObject.SetActive(localRole.canKill && !udiededed);
                //__instance.KillButton.canInteract = __instance.KillButton.enabled = __instance.KillButton.gameObject.active;
                if (__instance.KillButton.gameObject.active)
                {
                    float fullCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    TimeSpan timeLeft = DateTime.UtcNow - lastKillThingForCustoms;
                    int timeRemaining = (int)Math.Ceiling((double)new decimal(fullCooldown - timeLeft.TotalMilliseconds / 1000f));
                    __instance.KillButton.SetCoolDown(timeRemaining < 0 ? 0 : timeRemaining, fullCooldown);
                    __instance.KillButton.SetTarget(DillyzUtil.getClosestPlayer(PlayerControl.LocalPlayer));
                }
            }
        }

        public static void displayColor(HudManager __instance, PlayerControl player, Color roleColor) {
            string hex = DillyzUtil.colorToHex(roleColor);
            TextMeshPro tmp = player.gameObject.transform.Find("Names").Find("NameText_TMP").GetComponent<TextMeshPro>();
            tmp.text = $"<{hex}>{player.name}</color>";

            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                    if (pva.TargetPlayerId == player.PlayerId) {
                        pva.NameText.text = $"<{hex}>{player.name}</color>";
                        return;
                    }
        }
    }
}
