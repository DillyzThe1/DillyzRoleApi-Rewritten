﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DillyzRoleApi_Rewritten
{
    class SabotageBehaviour
    {
        public static bool commsDown = false;
        public static bool oxygenDown = false;
        public static bool reactorDown = false;
        [Obsolete("I literally can't find the light's system type! Sorry!", true)]
        public static bool lightsDown = false; // da da da duhhhhhh do da da de de da duhhhhhhh

        public static HudOverrideSystemType comms;
        public static LifeSuppSystemType oxygen;
        public static ReactorSystemType reactor;


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
    }

}
