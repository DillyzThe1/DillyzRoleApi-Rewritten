using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    // first ampong us mod code of 2023 :)))))
    class HostLocalGameButtonPatch
    {
        [HarmonyPatch(typeof(HostLocalGameButton), nameof(HostLocalGameButton.Start))]
        class HostLocalGameButtonPatch_Start
        {
            public static void Postfix(HostLocalGameButton __instance)
            {
                Transform t = __instance.transform.Find("CreateHnSGameButton");
                if (t != null && t.gameObject != null)
                    GameObject.Destroy(t.gameObject);
            }
        }
        [HarmonyPatch(typeof(HostLocalGameButton), nameof(HostLocalGameButton.ClickHideNSeek))]
        class HostLocalGameButtonPatch_ClickHideNSeek
        {
            public static bool Prefix(HostLocalGameButton __instance)
            {
                return false;
            }
        }
    }
}
