using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    class ExileControllerPatch
    {
        public static int initial_shifters = 0;
        public static int initial_engineers = 0;
        public static int initial_scientists = 0;
        public static void Postfix(GameData.PlayerInfo exiled, ExileController __instance)
        {
            if (exiled == null)
                return;

            if (!GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor)
            {
                __instance.completeString = $"{exiled.PlayerName} was ejected.";
                return;
            }

            string rolename = DillyzUtil.getRoleName(DillyzUtil.findPlayerControl(exiled.PlayerId));
            CustomRole role = CustomRole.getByName(rolename);

            if (role == null)
            {
                switch (rolename) {
                    case "ShapeShifter":
                        __instance.completeString = $"{exiled.PlayerName} was {(initial_shifters > 1 ? "a" : "The")} Shape Shifter.";
                        return;
                    case "Engineer":
                        __instance.completeString = $"{exiled.PlayerName} was {(initial_engineers > 1 ? "a" : "The")} Engineer.";
                        return;
                    case "Scientist":
                        __instance.completeString = $"{exiled.PlayerName} was {(initial_scientists > 1 ? "a" : "The")} Scientist.";
                        return;
                    case "Guardian Angel":
                        __instance.completeString = $"{exiled.PlayerName} was a Guardian Angel... somehow?";
                        return;
                }
                return;
            }

            if (role.curActive > 1)
                __instance.completeString = role.ejectionText.Replace("The", role.a_or_an).Replace("the", role.a_or_an).Replace("[0]", exiled.PlayerName);
            else
                __instance.completeString = role.ejectionText.Replace("[0]", exiled.PlayerName);
        }
    }
}
 