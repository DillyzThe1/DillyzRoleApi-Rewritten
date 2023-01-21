using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    class MeetingHudPatch
    {
        public static void Postfix() {
            CustomButton.ResetAllButtons();
        }
    }
}
