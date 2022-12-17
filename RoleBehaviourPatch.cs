using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    class RoleBehaviourPatch
    {
        /*[HarmonyPatch(typeof(RoleBehaviour), nameof(RoleBehaviour.DidWin))]
        public class RoleBehaviourPatch_SelectRoles
        {
            // override return value
            public static bool Prefix(RoleBehaviour __instance, GameOverReason gameOverReason, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("___JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }*/
        

        /*[HarmonyPatch(typeof(RoleBehaviour), "IsImpostor", MethodType.Getter)]
        public class RoleBehaviourPatch_IsImpostor
        {
            // override return value
            public static bool Prefix(RoleBehaviour __instance, ref bool __result)
            {
                if (__instance == null || __instance.Player == null)
                    return true;
                if (GameOverPatch.jesterWon)
                {
                    string nameeee = DillyzUtil.getRoleName(__instance.Player);
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!? " + nameeee + " " + __instance.Player.name);
                    __result = (nameeee == "Jester");
                }
                else
                    __result = DillyzUtil.roleSide(__instance.Player) == CustomRoleSide.Impostor;
                __result = true;
                return false;
            }
        }*/

        /*[HarmonyPatch(typeof(EngineerRole), nameof(EngineerRole.IsImpostor), MethodType.Getter)]
        public class EngineerRolePatch_IsImpostor
        {
            // override return value
            public static bool Prefix(EngineerRole __instance, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ScientistRole), nameof(ScientistRole.IsImpostor), MethodType.Getter)]
        public class ScientistRolePatch_IsImpostor
        {
            // override return value
            public static bool Prefix(ScientistRole __instance, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ShapeshifterRole), nameof(ShapeshifterRole.IsImpostor), MethodType.Getter)]
        public class ShapeshifterRolePatch_IsImpostor
        {
            // override return value
            public static bool Prefix(ShapeshifterRole __instance, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsImpostor), MethodType.Getter)]
        public class ImpostorRolePatch_IsImpostor
        {
            // override return value
            public static bool Prefix(ImpostorRole __instance, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CrewmateRole), nameof(CrewmateRole.IsImpostor), MethodType.Getter)]
        public class CrewmateRolePatch_IsImpostor
        {
            // override return value
            public static bool Prefix(CrewmateRole __instance, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(GuardianAngelRole), nameof(GuardianAngelRole.IsImpostor), MethodType.Getter)]
        public class GuardianAngelRolePatch_IsImpostor
        {
            // override return value
            public static bool Prefix(GuardianAngelRole __instance, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CrewmateGhostRole), nameof(CrewmateGhostRole.IsImpostor), MethodType.Getter)]
        public class CrewmateGhostRolePatch_IsImpostor
        {
            // override return value
            public static bool Prefix(CrewmateGhostRole __instance, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ImpostorGhostRole), nameof(ImpostorGhostRole.IsImpostor), MethodType.Getter)]
        public class ImpostorGhostRolePatch_IsImpostor
        {
            // override return value
            public static bool Prefix(ImpostorGhostRole __instance, ref bool __result)
            {
                if (GameOverPatch.jesterWon && __instance != null && __instance.Player != null)
                {
                    HarmonyMain.Instance.Log.LogInfo("JESTER WIN!?!?!!?");
                    __result = (DillyzUtil.getRoleName(__instance.Player) == "Jester");
                    return false;
                }
                return true;
            }
        }*/
    }
}
