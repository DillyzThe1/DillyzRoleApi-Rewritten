﻿using System;
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

            if (role.curActive > 1)
                __instance.completeString = role.ejectionText.Replace("The", role.a_or_an).Replace("the", role.a_or_an).Replace("[0]", exiled.PlayerName);
            else
                __instance.completeString = role.ejectionText.Replace("[0]", exiled.PlayerName);
        }
    }
}
 