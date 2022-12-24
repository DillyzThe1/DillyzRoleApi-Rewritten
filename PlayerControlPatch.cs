﻿using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DillyzRoleApi_Rewritten
{
    class PlayerControlPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class PlayerControlPatch_MurderPlayer {
            public static bool Prefix(PlayerControl __instance, PlayerControl target) {
                HarmonyMain.Instance.Log.LogWarning($"{__instance.name} tried to kill {target.name} using the normal PlayerControl.MurderPlayer() function!" +
                                                                                                $" (Did you forget to do DillyzUtil.RpcCommitAssassination()?)");
                DillyzUtil.commitAssassination(__instance, target);
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics._CoSpawnPlayer_d__49), nameof(PlayerPhysics._CoSpawnPlayer_d__49.MoveNext))]
        public class PlayerPhysicsPatch_CoSpawnPlayer
        {
            public static void Postfix(PlayerPhysics._CoSpawnPlayer_d__49 __instance, ref bool __result)
            {
                if (!AmongUsClient.Instance.AmHost || __instance.__4__this.name == "Player(Clone)")
                    return;

                HarmonyMain.Instance.Log.LogInfo("bruh " + __instance.__4__this.name);

                LobbyConfigManager.UpdateRoleValues();
            }
        }
    }
}
