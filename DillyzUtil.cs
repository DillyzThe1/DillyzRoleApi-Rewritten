﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;

namespace DillyzRoleApi_Rewritten
{
    class DillyzUtil
    {
        [Obsolete("Due to adding a set function, roleName() is now getRoleName().")]
        public static String roleName(PlayerControl player) {
            return getRoleName(player);
        }

        public static String getRoleName(PlayerControl player)
        {
            if (CustomRole.getRoleName(player.PlayerId) != "")
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
    }
}
