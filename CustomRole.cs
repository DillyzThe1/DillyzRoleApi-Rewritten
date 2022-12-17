using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// the idea here is that you'll instiate this class for your own purposes
// example for this:
//     CustomRole.createRole("Jester", "Get voted out to win. You're a real trickster!", true, new Color(90, 50, 200), false, LoneWolf, false, false);
namespace DillyzRoleApi_Rewritten
{
    class CustomRole
    {
        public static List<CustomRole> allRoles => new List<CustomRole>();
        public static void appendRole(CustomRole yourRole) => allRoles.Add(yourRole);
        public static void createRole(String name, String subtext, bool nameColor, Color roleColor, bool canSeeTeam, CustomRoleSide side,
                    bool canVent, bool canKill) {

            foreach (CustomRole role in allRoles)
                if (role.name == name)
                {
                    HarmonyMain.Instance.Log.LogError($"Role by name \"{role.name}\" already exists!");
                    return;
                }
            CustomRole.appendRole(new CustomRole(name, subtext, nameColor, roleColor, canSeeTeam, side, canVent, canKill));
        }
        public static CustomRole getByName(String name)
        {
            foreach (CustomRole role in allRoles)
                if (role.name == name)
                    return role;
            return null;
        }

        public String name = "Role Text";                       // Your role's name.
        public String subtext;                                  // The text that appears under.
        public bool nameColorChanges;                           // Determines if your name color is your role color or just red/white.
        public Color roleColor;                                 // The current color of your role.
        public bool teamCanSeeYou;                              // Determines if your team can see you.
        public CustomRoleSide side;                             // Determines who you work with.
        //public bool commitsTaxFraud { get; }                  // 192.512.3.62
        public bool canVent;                                    // If you can use the vents or not. You're an idiot for reading this text.
        public bool canKill;                                    // Are you seriously this blind?

        public CustomRole(String name, String subtext, bool nameColor, Color roleColor, bool canSeeTeam, CustomRoleSide side, bool canVent, bool canKill) {
            this.name = name;
            this.subtext = subtext;
            this.nameColorChanges = nameColor;
            this.roleColor = roleColor;
            this.teamCanSeeYou = canSeeTeam;
            this.side = side;
            this.canVent = canVent;
            this.canKill = canKill;
        }   
    }

    internal enum CustomRoleSide { 
        Impostor = 0,     // You work alongside the Impostors.
        Crewmate = 1,     // You work alongside the Crewmates.
        Independent = 2,  // You work upon your own team.
        LoneWolf = 3      // You work by yourself.
    }
}
