using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    class ExileControllerPatch
    {
        public static void Postfix(GameData.PlayerInfo exiled, ExileController __instance)
        {
            if (exiled == null)
                return;
            CustomRole role = CustomRole.getByName(DillyzUtil.getRoleName(DillyzUtil.findPlayerControl(exiled.PlayerId)));

            if (role == null || !GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor)
                return;

            __instance.completeString = role.ejectionText.Replace("[0]", exiled.PlayerName);
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    class PlayerExilePatch
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (HarmonyMain.Instance.enableDebugJester.Value && DillyzUtil.getRoleName(__instance) == "Jester")
                CustomRole.getByName("Jester").WinGame(__instance);
        }
    }
}
