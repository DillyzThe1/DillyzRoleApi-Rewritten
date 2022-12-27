using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    class GameStartManagerPatch
    {
        public static void Postfix(GameStartManager __instance) {
            __instance.MakePublicButton.gameObject.SetActive(false);
        }
    }
}
