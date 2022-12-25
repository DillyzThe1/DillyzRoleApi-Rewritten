using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    class GameStartManagerPatch
    {
        public static void Postfix(GameStartManager __instance) {
            __instance.MakePublicButton.gameObject.SetActive(false);
        }
    }
}
