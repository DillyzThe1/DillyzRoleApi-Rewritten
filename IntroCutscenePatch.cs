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
        public static void Postfix(IntroCutscene._ShowRole_d__35 __instance) {
            CustomRole role = CustomRole.getByName(DillyzUtil.getRoleName(PlayerControl.LocalPlayer));
            if (role == null)
                return;
            string colorHex = DillyzUtil.colorToHex(role.roleColor);
            HarmonyMain.Instance.Log.LogInfo(role.roleColor + " " + colorHex);
            __instance.__4__this.RoleText.text = $"<{colorHex}>{role.name}</color>";
            __instance.__4__this.RoleBlurbText.text = $"<{colorHex}>{role.subtext}</color>";
            __instance.__4__this.YouAreText.text = $"<{colorHex}>Your role is</color>";
        }
    }
}
