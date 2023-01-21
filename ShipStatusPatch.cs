using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
    class ShipStatusPatch
    {
        public static void Prefix(ShipStatus __instance) {
            //if (DillyzUtil.InGame() && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Ended)
            //CustomRole.allRoleNames.Clear();
            if (DillyzUtil.InFreeplay())
                DillyzRoleApiMain.ResetStuffForLobby();
        }
    }


    // why does this double all cooldowns?!
    /*[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    class ShipStatusPatch_Begin
    {
        public static void Prefix(ShipStatus __instance)
        {
            CustomButton.ResetAllButtons();
        }
    }*/
}
