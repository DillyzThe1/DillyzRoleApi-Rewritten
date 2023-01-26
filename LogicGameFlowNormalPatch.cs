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
                if (SabotageBehaviour.oxygen != null && !SabotageBehaviour.oxygen.WasCollected)
                {
                    if (SabotageBehaviour.oxygen.Countdown < 0f)
                    {
                        __instance.EndGameForSabotage();
                        SabotageBehaviour.oxygen.Countdown = 10000f;
                        return false;
                    }
                }
                if (SabotageBehaviour.reactor != null && !SabotageBehaviour.reactor.WasCollected)
                {
                    if (SabotageBehaviour.reactor.Countdown < 0f)
                    {
                        __instance.EndGameForSabotage();
                        SabotageBehaviour.reactor.ClearSabotage();
                        return false;
                    }
                }

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
