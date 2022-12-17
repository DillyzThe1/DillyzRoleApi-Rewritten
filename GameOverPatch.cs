using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    class GameOverPatch
    {
        public static bool jesterWon = false;
        public static void Prefix(EndGameManager __instance) {
            if (jesterWon) {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls) 
                    if (DillyzUtil.getRoleName(DillyzUtil.findPlayerControl(player.PlayerId)) != "Jester")
                        player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                    else
                        player.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;

                string hexthing = DillyzUtil.colorToHex(CustomRole.getByName("Jester").roleColor);
                if (DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "Jester")
                    __instance.WinText.text = $"<{hexthing}>Victory</color>";
                else
                    __instance.WinText.text = $"<{hexthing}>Defeat</color>";
                __instance.WinText.material.color = CustomRole.getByName("Jester").roleColor;
                __instance.BackgroundBar.material.color = CustomRole.getByName("Jester").roleColor;
            }
        }
    }
}
