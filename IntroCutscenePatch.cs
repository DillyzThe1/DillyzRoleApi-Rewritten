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
    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__35), nameof(IntroCutscene._ShowRole_d__35.MoveNext))]
    class IntroCutscenePatch
    {
        public static string colorHex = "#FF0000";
        public static void Postfix(IntroCutscene._ShowRole_d__35 __instance) {
            GameOverPatch.customWin = false;

            CustomRole role = CustomRole.getByName(DillyzUtil.getRoleName(PlayerControl.LocalPlayer));
            if (role == null)
            {
                if (DillyzUtil.roleSide(PlayerControl.LocalPlayer) == CustomRoleSide.Crewmate)
                    colorHex = DillyzUtil.colorToHex(CustomPalette.CrewmateBlue);
                else
                    colorHex = DillyzUtil.colorToHex(CustomPalette.ImpostorRed);

                HarmonyMain.Instance.Log.LogInfo((DillyzUtil.roleSide(PlayerControl.LocalPlayer) == CustomRoleSide.Crewmate) + " " + colorHex);
                return;
            }

            colorHex = DillyzUtil.colorToHex(role.roleColor);
            HarmonyMain.Instance.Log.LogInfo(role.roleColor + " " + colorHex);
            __instance.__4__this.RoleText.text = $"<{colorHex}>{role.name}</color>";
            __instance.__4__this.RoleBlurbText.text = $"<{colorHex}>{role.subtext}</color>";
            __instance.__4__this.YouAreText.text = $"<{colorHex}>Your role is</color>";
        }
    }
    [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__32), nameof(IntroCutscene._ShowTeam_d__32.MoveNext))]
    class IntroCutscenePatch_Team
    {
        public static void Postfix(IntroCutscene._ShowRole_d__35 __instance)
        {
            CustomRole role = CustomRole.getByName(DillyzUtil.getRoleName(PlayerControl.LocalPlayer));
            if (role == null)
                return;

            if (role.side == CustomRoleSide.Impostor || role.side == CustomRoleSide.Crewmate)
                return;

            Color intendedColor = (role.side == CustomRoleSide.Independent) ? role.roleColor : CustomPalette.LoneWolfGray;
            string colorHex = DillyzUtil.colorToHex(intendedColor);
            string teamText = (role.side == CustomRoleSide.Independent) ? role.name : "Neutral";
            __instance.__4__this.TeamTitle.text = $"<{colorHex}>{teamText}</color>";
            __instance.__4__this.ImpostorText.text = (role.side == CustomRoleSide.Independent) ? role.name : 
                                                        $"You have <{DillyzUtil.colorToHex(CustomPalette.ImpostorRed)}>no team</color>.";
            __instance.__4__this.BackgroundBar.material.color = intendedColor;
        }
    }

    // moved 
    /*// force it to update
    [HarmonyPatch(typeof(TextMeshPro), nameof(TextMeshPro.InternalUpdate))]
    class TMPPROPatch
    {
        public static void Postfix(TextMeshPro __instance)
        {
            if (__instance.name == "YouAreText")
                __instance.text = $"<{IntroCutscenePatch.colorHex}>Your role is</color>";
        }
    }*/
}
