using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    class GameModeMenuPatch
    {
        [HarmonyPatch(typeof(GameModeMenu), nameof(GameModeMenu.OnEnable))]
        public class GameModeMenuPatch_OnEnable {
            public static void Postfix(GameModeMenu __instance) {
                __instance.transform.parent.gameObject.SetActive(false);
                __instance.ChooseOption(AmongUs.GameOptions.GameModes.Normal);
            }
        }
    }
}
