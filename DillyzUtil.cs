using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using Hazel;
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
                    return "ShapeShifter";
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

            return player.Data.Role.IsImpostor ? CustomPalette.ImpostorRed : (nameText ? CustomPalette.White : CustomPalette.CrewmateBlue);
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
        public static String colorToHex(Color ogColor) => "#" + convNumToHexNum((int)Math.Floor(ogColor.r)) +
                                                                convNumToHexNum((int)Math.Floor(ogColor.g)) + convNumToHexNum((int)Math.Floor(ogColor.b));


        public static PlayerControl getClosestPlayer(PlayerControl centerPlayer)
        {
            return getClosestPlayer(centerPlayer, GameOptionsManager.Instance.currentNormalGameOptions.KillDistance);
        }
        public static PlayerControl getClosestPlayer(PlayerControl centerPlayer, double mindist)
        {
            return getClosestPlayer(centerPlayer, null, mindist, true);
        }

        public static PlayerControl getClosestPlayer(PlayerControl centerPlayer, List<String> roleFilters, double mindist, bool shouldBeAlive) {
            List<PlayerControl> welcomeOldPlayers = PlayerControl.AllPlayerControls.ToArray().ToList();
            PlayerControl close = null;
            double playerDist = mindist;

            if (roleFilters != null)
                welcomeOldPlayers.RemoveAll(x => roleFilters.Contains(getRoleName(x)));
            welcomeOldPlayers.RemoveAll(x => x.Data.IsDead == shouldBeAlive);

            foreach (PlayerControl player in welcomeOldPlayers)
            {
                double dist = getDistBetweenPlayers(centerPlayer, player);
                if (dist >= playerDist)
                    continue;
                playerDist = dist;
                close = player;
            }
            return close;
        }

        // A workaround for killing.
        public static void RpcCommitAssassination(PlayerControl assassinator, PlayerControl target)
        {
            string rolename = getRoleName(assassinator);

            if (rolename == "Impostor" || rolename == "ShapeShifter")
            {
                assassinator.RpcMurderPlayer(target);
                return;
            }

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.Assassinate, Hazel.SendOption.None, -1);
            writer.Write(assassinator.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            commitAssassination(assassinator, target);
        }

        public static void commitAssassination(PlayerControl assassinator, PlayerControl target) {
            RoleTypes oldroletype = assassinator.Data.RoleType;
            RoleBehaviour oldrole = assassinator.Data.Role;

            assassinator.Data.RoleType = RoleTypes.Impostor;
            assassinator.Data.Role = new ImpostorRole();

            assassinator.MurderPlayer(target);

            assassinator.Data.RoleType = oldroletype;
            assassinator.Data.Role = oldrole;
        } 
        
        public static double getDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            Vector2 refpos = refplayer.GetTruePosition();
            Vector2 playerpos = player.GetTruePosition();
            // maths
            return Math.Sqrt((refpos[0] - playerpos[0]) * (refpos[0] - playerpos[0]) + (refpos[1] - playerpos[1]) * (refpos[1] - playerpos[1]));
        }
    }
}
