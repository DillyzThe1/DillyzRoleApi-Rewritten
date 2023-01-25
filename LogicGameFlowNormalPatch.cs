using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    class LogicGameFlowNormalPatch
    {
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
        public class LogicGameFlowNormalPatch_IsGameOverDueToDeath
        {
            public static bool Prefix(LogicGameFlowNormal __instance, ref bool __result)
            {
                foreach (CustomRole role in CustomRole.allRoles)
                    if (role.returnWinConditionState != null)
                    {
                        switch (role.returnWinConditionState())
                        {
                            case WinConditionState.None:
                                break;
                            case WinConditionState.GameOver:
                                __result = true;
                                role.WinGame(role.rwcsPlayer());
                                return false;
                            case WinConditionState.Hold:
                                __result = false;
                                return false;
                        }
                    }

                return true;
            }
        }

        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
        public class LogicGameFlowNormalPatch_CheckEndCriteria
        {
            public static bool Prefix(LogicGameFlowNormal __instance)
            {
                if (!GameData.Instance)
                    return false;

                // SABOTAGE WINS (no force check needed
                /*ISystemType systemType;
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
                }*/

                foreach (CustomRole role in CustomRole.allRoles)
                    if (role.returnWinConditionState != null)
                    {
                        switch (role.returnWinConditionState())
                        {
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
}
