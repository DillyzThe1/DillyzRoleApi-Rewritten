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
        static void Postfix(IntroCutscene._ShowRole_d__35 __instance) {
            HarmonyMain.Instance.Log.LogInfo("YOOO I'M THE " + __instance.__4__this.TeamTitle.text);
            HarmonyMain.Instance.Log.LogInfo("2 THE " + __instance.__4__this.RoleText.text);
            HarmonyMain.Instance.Log.LogInfo("3 THE " + __instance.__4__this.RoleBlurbText.text);
            HarmonyMain.Instance.Log.LogInfo("4 THE " + __instance.__4__this.BackgroundBar.material.color);
            HarmonyMain.Instance.Log.LogInfo("5 THE " + __instance.__4__this.ImpostorName.text);
            HarmonyMain.Instance.Log.LogInfo("6 THE " + __instance.__4__this.ImpostorText.text);
            HarmonyMain.Instance.Log.LogInfo("7 THE " + __instance.__4__this.ImpostorTitle.text);
            HarmonyMain.Instance.Log.LogInfo("8 THE " + __instance.__4__this.YouAreText.text);

            __instance.__4__this.TeamTitle.text = "team title";
            __instance.__4__this.RoleText.text = "role text";
            __instance.__4__this.RoleBlurbText.text = "role blurb text";
            __instance.__4__this.BackgroundBar.material.color = new Color(255, 0, 0);
            __instance.__4__this.ImpostorName.text = "impostor name";
            __instance.__4__this.ImpostorText.text = "impostor text";
            __instance.__4__this.ImpostorTitle.text = "impostor title";
            __instance.__4__this.YouAreText.text = "You're*";
        }
    }
}
