using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
    class ShipStatusPatch
    {
        public static void Prefix(ShipStatus __instance) {
            if (DillyzUtil.InFreeplay())
                DillyzRoleApiMain.ResetStuffForLobby();
        }
    }
}
