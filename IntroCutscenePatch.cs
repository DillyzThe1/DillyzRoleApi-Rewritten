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
                colorHex = DillyzUtil.colorToHex(DillyzUtil.roleColor(PlayerControl.LocalPlayer, false));
                __instance.__4__this.RoleText.text = $"<{colorHex}>{__instance.__4__this.RoleText.text}</color>";
                __instance.__4__this.RoleBlurbText.text = $"<{colorHex}>{__instance.__4__this.RoleBlurbText.text}</color>";
                __instance.__4__this.YouAreText.text = $"<{colorHex}>Your role is</color>";
                //HarmonyMain.Instance.Log.LogInfo((DillyzUtil.roleSide(PlayerControl.LocalPlayer) == CustomRoleSide.Crewmate) + " " + colorHex);
                return;
            }

            colorHex = DillyzUtil.colorToHex(role.roleColor);
            HarmonyMain.Instance.Log.LogInfo(role.roleColor + " " + colorHex);
            __instance.__4__this.RoleText.text = $"<{colorHex}>{role.name}</color>";
            __instance.__4__this.RoleBlurbText.text = $"<{colorHex}>{role.subtext}</color>";
            __instance.__4__this.YouAreText.text = $"<{colorHex}>Your role is</color>";

            HudManager.Instance.SetHudActive(true);
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

            Color32 intendedColor = (role.side == CustomRoleSide.Independent) ? role.roleColor : CustomPalette.LoneWolfGray;
            string colorHex = DillyzUtil.colorToHex(intendedColor);
            string teamText = (role.side == CustomRoleSide.Independent) ? role.name : "Neutral";
            __instance.__4__this.TeamTitle.text = $"<{colorHex}>{teamText}</color>";
            __instance.__4__this.ImpostorText.text = (role.side == CustomRoleSide.Independent) ? role.name : 
                                                        $"You have <{DillyzUtil.colorToHex(CustomPalette.ImpostorRed)}>no team</color>.";
            __instance.__4__this.BackgroundBar.material.color = intendedColor;
        }
    }

    //[HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    class IntroCutscenePatch_BeginImpostor
    {
        public static bool Prefix(IntroCutscene __instance, Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            HarmonyMain.Instance.Log.LogInfo("Flabbergasted Impostor");
            IntroCutscenePatch_BeginImpostor.calcTeam();
            __instance.ShowTeam(IntroCutscenePatch_BeginImpostor.playersToShow, 3);
            return false;
        }

        public static Il2CppSystem.Collections.Generic.List<PlayerControl> playersToShow;
        public static void calcTeam() {
            List<PlayerControl> playersToShowww = PlayerControl.AllPlayerControls.ToArray().ToList();

            CustomRoleSide rs = DillyzUtil.roleSide(PlayerControl.LocalPlayer);

            switch (rs)
            {
                case CustomRoleSide.Impostor:
                    playersToShowww.RemoveAll(x => DillyzUtil.roleSide(x) != CustomRoleSide.Impostor);
                    break;
                case CustomRoleSide.Independent:
                    playersToShowww.RemoveAll(x => DillyzUtil.getRoleName(x) != DillyzUtil.getRoleName(PlayerControl.LocalPlayer));
                    break;
                case CustomRoleSide.LoneWolf:
                    playersToShowww.RemoveAll(x => x != PlayerControl.LocalPlayer);
                    break;
            }

            playersToShow = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            // conversion bc dumb
            foreach (PlayerControl player in playersToShowww)
                playersToShow.Add(player);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowTeam))]
    class IntroCutscenePatch_ShowTeam
    {
        public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToShow, float duration)
        {
            HarmonyMain.Instance.Log.LogInfo("Flabbergasted Team " + teamToShow.ToString() + " " + duration);
            foreach (PlayerControl i in teamToShow)
                HarmonyMain.Instance.Log.LogInfo(i.name + " is FLABBERGASTED right now!");

            IntroCutscenePatch_BeginImpostor.calcTeam();

            teamToShow = IntroCutscenePatch_BeginImpostor.playersToShow;
        }

    }
}
