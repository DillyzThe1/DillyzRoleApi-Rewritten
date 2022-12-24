using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    class LogicGameFlowNormalPatch
    {
        public static bool Prefix(LogicGameFlowNormal __instance) {
            if (!GameData.Instance)
                return false;

            // SABOTAGE WINS (no force check needed
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.LifeSupp, out systemType))
            {
                LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                if (lifeSuppSystemType.Countdown < 0f)
                {
                    __instance.EndGameForSabotage();
                    lifeSuppSystemType.Countdown = 10000f;
                    return false;
                }
            }
            ISystemType systemType2;
            if ((ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Reactor, out systemType2) || ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Laboratory, out systemType2)) && systemType2.TryCast<ICriticalSabotage>() is ICriticalSabotage)
            {
                ICriticalSabotage criticalSabotage = systemType2.TryCast<ICriticalSabotage>();
                if (criticalSabotage.Countdown < 0f)
                {
                    __instance.EndGameForSabotage();
                    criticalSabotage.ClearSabotage();
                    return false;
                }
            }

            foreach (CustomRole role in CustomRole.allRoles)
                if (role.returnWinConditionState != null)
                {
                    switch (role.returnWinConditionState()) {
                        case WinConditionState.None:
                            break;
                        case WinConditionState.GameOver:
                            role.WinGame(role.rwcsPlayer());
                            break;
                        case WinConditionState.Hold:
                            return false;
                    }
                }

            return true;
        }
    }
}
