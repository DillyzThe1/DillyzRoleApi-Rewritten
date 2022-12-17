using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
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

            HarmonyMain.Instance.Log.LogInfo(role.roleColor);
            __instance.__4__this.RoleText.text = role.name;
            __instance.__4__this.RoleBlurbText.text = role.subtext;
            __instance.__4__this.RoleText.material.color = role.roleColor;
            __instance.__4__this.RoleBlurbText.material.color = role.roleColor;
            __instance.__4__this.YouAreText.material.color = role.roleColor;
        }
    }
}
