using HarmonyLib;
using System;

namespace DillyzRoleApi_Rewritten
{
    class SabotageBehaviour
    {
        public static bool commsDown = false;
        public static bool oxygenDown = false;
        public static bool reactorDown = false;
        [Obsolete("I literally can't find the light's system type! Sorry!", true)]
        public static bool lightsDown = false; // da da da duhhhhhh do da da de de da duhhhhhhh
        public static PlainDoor[] AllDoors => (ShipStatus.Instance != null ? ShipStatus.Instance.AllDoors : new PlainDoor[] { });

        public static HudOverrideSystemType comms;
        public static LifeSuppSystemType oxygen;
        public static ReactorSystemType reactor;
        [Obsolete("Unsupported due to instability.", true)]
        public static DoorsSystemType doors;


        [HarmonyPatch(typeof(HudOverrideSystemType), nameof(HudOverrideSystemType.IsActive), MethodType.Getter)]
        class HudOverrideSystemTypePatch_IsActive_get
        {
            public static void Postfix(HudOverrideSystemType __instance, ref bool __result)
            {
                commsDown = __result;
                comms = __instance;
            }
        }

        [HarmonyPatch(typeof(LifeSuppSystemType), nameof(LifeSuppSystemType.IsActive), MethodType.Getter)]
        class LifeSuppSystemType_IsActive_get
        {
            public static void Postfix(LifeSuppSystemType __instance, ref bool __result)
            {
                oxygenDown = __result;
                oxygen = __instance;
            }
        }

        [HarmonyPatch(typeof(ReactorSystemType), nameof(ReactorSystemType.IsActive), MethodType.Getter)]
        class ReactorSystemType_IsActive_get
        {
            public static void Postfix(ReactorSystemType __instance, ref bool __result)
            {
                reactorDown = __result;
                reactor = __instance;
            }
        }

        /*[HarmonyPatch(typeof(DoorsSystemType), nameof(DoorsSystemType.IsActive), MethodType.Getter)]
        class DoorsSystemType_IsActive_get
        {
            public static void Postfix(DoorsSystemType __instance, ref bool __result)
            {
                doors = __instance;
            }
        }*/
    }

}
