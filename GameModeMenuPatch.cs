using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DillyzRoleApi_Rewritten
{
    class GameModeMenuPatch
    {
        [HarmonyPatch(typeof(GameModeMenu), nameof(GameModeMenu.OnEnable))]
        public class GameModeMenuPatch_OnEnable {
            public static void Postfix(GameModeMenu __instance) {
                __instance.transform.parent.gameObject.SetActive(false);
                __instance.ChooseOption(AmongUs.GameOptions.GameModes.Normal);
            }
        }
    }
}
