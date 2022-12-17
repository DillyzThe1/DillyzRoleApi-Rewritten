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
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerPatch
    {
        public static void Postfix(HudManager __instance)
        {
            CustomRole localRole = CustomRole.getByName(DillyzUtil.getRoleName(PlayerControl.LocalPlayer));
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                CustomRole theRole = CustomRole.getByName(DillyzUtil.getRoleName(player));

                // if the role is not there or doesn't change color, leave it alone
                if (theRole == null || !theRole.nameColorChanges)
                {
                    CustomRoleSide rs = DillyzUtil.roleSide(player);
                    CustomRoleSide rs2 = DillyzUtil.roleSide(PlayerControl.LocalPlayer);
                    HudManagerPatch.displayColor(__instance, player, (rs == CustomRoleSide.Impostor && rs == rs2) ? CustomPalette.ImpostorRed : CustomPalette.White);
                    continue;
                }

                 if ((player.PlayerId == PlayerControl.LocalPlayer.PlayerId) ||  // If you're the player.
                    theRole.nameColorPublic ||  // If the name color was public.
                    // If it's private but you get it.
                    (theRole.teamCanSeeYou && localRole != null && theRole.name == localRole.name && theRole.side != CustomRoleSide.LoneWolf))
                    HudManagerPatch.displayColor(__instance, player, theRole.roleColor);
                else
                    HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
            }
        }

        public static void displayColor(HudManager __instance, PlayerControl player, Color roleColor)//CustomRole curRole)
        {
            /*Color intendedColor = (curRole == null) ?
                (DillyzUtil.roleSide(player) == CustomRoleSide.Impostor ? Palette.ImpostorRed : Palette.White)  : curRole.roleColor;*/

            string hex = DillyzUtil.colorToHex(roleColor);
            TextMeshPro tmp = player.gameObject.transform.Find("Names").Find("NameText_TMP").GetComponent<TextMeshPro>();
            tmp.text = $"<{hex}>{player.name}</color>";
            //tmp.text += $"\n<{hex}>{curRole.name}</color>";

            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                    if (pva.TargetPlayerId == player.PlayerId) {
                        pva.NameText.text = $"<{hex}>{player.name}</color>";
                        return;
                    }
        }
    }
}
