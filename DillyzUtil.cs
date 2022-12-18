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
            if (player == null || player.Data == null)
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
            if (player == null || player.Data == null)
                return CustomRoleSide.Crewmate;
            if (CustomRole.getRoleName(player.PlayerId) != "" && CustomRole.getRoleName(player.PlayerId) != null)
                return CustomRole.getByName(CustomRole.getRoleName(player.PlayerId)).side;
            if (player.Data.RoleType == RoleTypes.Impostor || player.Data.RoleType == RoleTypes.ImpostorGhost || player.Data.RoleType == RoleTypes.Shapeshifter)
                return CustomRoleSide.Impostor;
            return CustomRoleSide.Crewmate;
        }

        public static Color roleColor(PlayerControl player, bool nameText)
        {
            if (player == null || player.Data == null)
                return nameText ? CustomPalette.White : CustomPalette.CrewmateBlue;
            if (CustomRole.getRoleName(player.PlayerId) != "" && CustomRole.getRoleName(player.PlayerId) != null)
                return CustomRole.getByName(CustomRole.getRoleName(player.PlayerId)).roleColor;

            switch (player.Data.RoleType) {
                case RoleTypes.Impostor | RoleTypes.ImpostorGhost:
                    return CustomPalette.ImpostorRed;
                case RoleTypes.Shapeshifter:
                    return CustomPalette.ShapeShifterCrimson;
                case RoleTypes.Engineer:
                    return CustomPalette.EngineerOrange;
                case RoleTypes.Scientist:
                    return CustomPalette.ScientistTeal;
                case RoleTypes.GuardianAngel:
                    return CustomPalette.GuardianAngleLightBlue;
            }

            return nameText ? CustomPalette.White : CustomPalette.CrewmateBlue;
        }

        public static string roleText(PlayerControl player)
        {
            if (player == null)
                return "";
            if (player == null || player.Data == null)
                return "You're a Crewmate.";
            string rc = colorToHex(roleColor(player, false));

            if (CustomRole.getRoleName(player.PlayerId) != "" && CustomRole.getRoleName(player.PlayerId) != null)
            {
                CustomRole cr = CustomRole.getByName(CustomRole.getRoleName(player.PlayerId));
                if (cr.name != "" && cr.name != null)
                    return $"\n\n<{rc}>You're {cr.a_or_an} {cr.name}. {cr.subtext}</color>";
            }

            switch (player.Data.RoleType)
            {
                case RoleTypes.Impostor | RoleTypes.ImpostorGhost:
                    return $"\n\n<{rc}>You're an Impostor.</color>";
                case RoleTypes.Shapeshifter:
                    return $"\n\n<{rc}>You're a Shapeshifter. Shapeshift to decieve the crew.</color>";
                case RoleTypes.Engineer:
                    return $"\n\n<{rc}>You're an Engineer. Use the vents to help the crew.</color>";
                case RoleTypes.Scientist:
                    return $"\n\n<{rc}>You're a Scientist. Use vitals to track the crew.</color>";
                case RoleTypes.GuardianAngel:
                    return $"\n\n<{rc}>You're a Guardian Angel. Protect the crew from kills.</color>";
            }

            return "You're a Crewmate.";
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
