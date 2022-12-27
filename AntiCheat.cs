using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    class AntiCheat
    {
        // screw ur bans lmao
        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
        public static class AmBannedPatch
        {
            public static void Postfix(out bool __result)
            {
                __result = false;
            }
        }

        [HarmonyPatch(typeof(FindGameButton), nameof(FindGameButton.OnClick))]
        public static class FindGameDisablePatch
        {
            public static bool Prefix(FindGameButton __instance)
            {
                AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.Custom;
                AmongUsClient.Instance.LastCustomDisconnect = "Cheating on regular servers is <#FF6A00>STRICTLY</color> prohibited.\n<#FF0000>Don't</color> attempt it.";
                AmongUsClient.Instance.HandleDisconnect(AmongUsClient.Instance.LastDisconnectReason, AmongUsClient.Instance.LastCustomDisconnect);
                return false;
            }
        }
    }
}
