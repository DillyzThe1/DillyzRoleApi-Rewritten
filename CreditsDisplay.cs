using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch]
    class CreditsDisplay
    {
        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        public static class VersionShowerPatch
        {
            public static void Postfix(VersionShower __instance)
            {
                __instance.text.text += $"\n<#F6FF00>{HarmonyMain.MOD_NAME}</color> v{HarmonyMain.MOD_VERSION} by <#3AA3D9>DillyzThe1</color>";
                HarmonyMain.Instance.Log.LogInfo("pos" + __instance.transform.position);
                __instance.transform.position = new Vector3(__instance.transform.position.x,
                                                            __instance.transform.position.y - 0.15f, __instance.transform.position.z);
            }
        }

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        class PingPatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.text.text += $"\n<#F6FF00>{HarmonyMain.MOD_NAME}</color> v{HarmonyMain.MOD_VERSION}\n<#3AA3D9>github.com/DillyzThe1</color>";
            }
        }
    }
}
