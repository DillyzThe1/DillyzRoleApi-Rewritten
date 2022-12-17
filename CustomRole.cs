using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// the idea here is that you'll override this class for your own roles
// example for overriding a field: public String name => "Jester";
namespace DillyzRoleApi_Rewritten
{
    class CustomRole
    {
        public String name { get; set; }             // Your role's name.
        public String subtext { get; set; }          // The text that appears under.
        public bool nameColorChanges { get; set; }   // Determines if your name color is your role color or just red/white.
        public Color roleColor { get; set; }         // The current color of your role.
        public bool teamCanSeeYou { get; set; }      // Determines if your team can see you.
        public CustomRoleSide side { get; set; }     // Determines who you work with.
        public bool commitsTaxFraud { get; set; }    // 192.512.3.62
        public bool canVent { get; set; }            // If you can use the vents or not. You're an idiot for reading this text.
        public bool canKill { get; set; }            // Are you seriously this blind?
        public bool canDoTasks { get; set; }         // No really, what is wrong with you? Stop reading these texts.
    }

    internal enum CustomRoleSide { 
        Impostor = 0,     // You work alongside the Impostors.
        Crewmate = 1,     // You work alongside the Crewmates.
        Independent = 2,  // You work upon your own team.
        LoneWolf = 3      // You work by yourself.
    }
}
