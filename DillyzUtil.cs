using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    class DillyzUtil
    {
        public static String getRoleName(PlayerControl player)
        {
            if (player == null)
                return "Crewmate";

            if (CustomRole.getRoleName(player.PlayerId) != "" && CustomRole.getRoleName(player.PlayerId) != null)
                return CustomRole.getRoleName(player.PlayerId);

            switch (player.Data.RoleType)
            {
                case RoleTypes.Crewmate | RoleTypes.CrewmateGhost:
                    return "Crewmate";
                case RoleTypes.Impostor | RoleTypes.ImpostorGhost:
                    return "Impostor";
                case RoleTypes.Scientist:
                    return "Scientist";
                case RoleTypes.Engineer:
                    return "Engineer";
                case RoleTypes.GuardianAngel:
                    return "GuardianAngel";
                case RoleTypes.Shapeshifter:
                    return "Shapeshifter";
            }

            return "Crewmate";
        }

        public static CustomRoleSide roleSide(PlayerControl player)
        {
            if (CustomRole.getRoleName(player.PlayerId) != "")
                return CustomRole.getByName(CustomRole.getRoleName(player.PlayerId)).side;
            if (player.Data.RoleType == RoleTypes.Impostor || player.Data.RoleType == RoleTypes.ImpostorGhost || player.Data.RoleType == RoleTypes.Shapeshifter)
                return CustomRoleSide.Impostor;
            return CustomRoleSide.Crewmate;
        }

        public static bool templateRole(PlayerControl player) {
            String name = getRoleName(player);
            return name == "Impostor" || name == "Crewmate";
        }

        public static PlayerControl findPlayerControl(byte playerId) {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == playerId)
                    return player;
            return null;
        }

        public static Color copyColor(Color ogColor) => new Color(ogColor.r, ogColor.g, ogColor.b);
        private static String convDigitToHexNum(int num) {
            if (num < 10)
                return num.ToString();
            switch (num % 16) {
                case 10: return "A";
                case 11: return "B";
                case 12: return "C";
                case 13: return "D";
                case 14: return "E";
                case 15: return "F";
            }
            return "0";
        }
        private static String convNumToHexNum(int num) => convDigitToHexNum(num / 16) + convDigitToHexNum(num % 16);
        public static String colorToHex(Color ogColor)  => "#" + convNumToHexNum((int)Math.Floor(ogColor.r)) + 
                                                                convNumToHexNum((int)Math.Floor(ogColor.g)) + convNumToHexNum((int)Math.Floor(ogColor.b));
    }
}
