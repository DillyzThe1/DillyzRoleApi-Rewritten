using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hazel;
using UnityEngine;

// the idea here is that you'll instiate this class for your own purposes
// example for this:
//     CustomRole.createRole("Jester", "Get voted out to win. You're a real trickster!", true, new Color32(90, 50, 200), false, LoneWolf, false, false);
namespace DillyzRoleApi_Rewritten
{
    public class CustomRole
    {
        public static List<CustomRole> allRoles = new List<CustomRole>();
        public static void appendRole(CustomRole yourRole) => allRoles.Add(yourRole);
        public static CustomRole createRole(String name, String subtext, bool nameColor, bool nameColorPublic, Color32 roleColor, bool canSeeTeam, CustomRoleSide side,
                    VentPrivilege ventPrivilege, bool canKill, bool showEjectText) {
            foreach (CustomRole role in allRoles)
                if (role.name == name)
                {
                    HarmonyMain.Instance.Log.LogError($"Role by name \"{role.name}\" already exists!");
                    return role;
                }
            CustomRole rolee = new CustomRole(name, subtext, nameColor, nameColorPublic, roleColor, canSeeTeam, side, ventPrivilege, canKill, showEjectText);
            CustomRole.appendRole(rolee);
            return rolee;
        }
        public static CustomRole getByName(String name)
        {
            foreach (CustomRole role in allRoles)
                if (role.name == name)
                    return role;
            return null;
        }

        // player id to role name
        public static Dictionary<byte, string> roleNameMap = new Dictionary<byte, string>();
        public static string getRoleName(byte playerId) => roleNameMap.ContainsKey(playerId) ? roleNameMap[playerId] : "";
        public static void setRoleName(byte playerId, string roleName) => roleNameMap[playerId] = roleName;

        public String name = "Role Text";                       // Your role's name.
        public String subtext;                                  // The text that appears under.
        public bool nameColorChanges;                           // Determines if your name color is your role color or just red/white.
        public bool nameColorPublic;                            // Determines if your name color is public to all or not.
        public Color32 roleColor;                                 // The current color of your role.
        public bool teamCanSeeYou;                              // Determines if your team can see you.
        public CustomRoleSide side;                             // Determines who you work with.
        //public bool commitsTaxFraud { get; }                  // 192.512.3.62
        public VentPrivilege ventPrivilege;                     // Your vent privelege level.
        public bool canKill;                                    // Are you seriously this blind?
        public string ejectionText;                             // "DillyzThe1 was The Jester"
        public bool switchToImpostor = false;                   // Will switch a crewmate role to an Impostor role.
        public string a_or_an = "an";                           // "DillyzThe1 was a Jester" vs "DillyzThe1 was an Jester"

        public CustomRole(String name, String subtext, bool nameColor, bool nameColorPublic, Color32 roleColor, bool canSeeTeam, 
                            CustomRoleSide side, VentPrivilege ventPrivilege, bool canKill, bool showEjectText) {
            this.name = name;
            this.subtext = subtext;
            this.nameColorChanges = nameColor;
            this.nameColorPublic = nameColorPublic;
            this.roleColor = roleColor;
            this.teamCanSeeYou = canSeeTeam;
            this.side = side;
            this.ventPrivilege = ventPrivilege;
            this.canKill = canKill;
            this.ejectionText = "[0] was ";
            //this.ejectionText_bad = "[0] was not ";

            // vs woudn't let me use turnary operators this time >:/
            if (showEjectText)
            {
                ejectionText += $"The {name}.";
                //ejectionText_bad += $"The <${DillyzUtil.colorToHex(this.roleColor)}>{name}.</color>";
            }
            else
            {
                ejectionText += $"The Impostor.";
                //ejectionText_bad += $"The Impostor.";
            }
        }

        public override string ToString() {
            return $"DillyzRoleApi_Rewritten.CustomRole [name: {this.name}, subtext: {this.subtext}, nameColorChanges: {this.nameColorChanges}, " +
                   $"roleColor: [{this.roleColor.r}, {this.roleColor.g}, {this.roleColor.b}, teamCanSeeYou: {this.teamCanSeeYou}, side: {this.side}, " +
                   $"ventPrivilege: {this.ventPrivilege}, canKill: {this.canKill}]";
        }

        public void WinGame(PlayerControl cause) {
            if (this.side == CustomRoleSide.Crewmate)
            {
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
                return;
            }
            if (this.side == CustomRoleSide.Impostor)
            {
                GameManager.Instance.RpcEndGame(GameOverReason.ImpostorByKill, false);
                return;
            }
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.CustomRoleWin, Hazel.SendOption.None, -1);
            writer.Write(this.name);
            writer.Write(cause.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            GameOverPatch.SetAllToWin(this.name, cause, true);
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorByKill, false);
        }
    }

    public enum CustomRoleSide { 
        Impostor = 0,     // You work alongside the Impostors.
        Crewmate = 1,     // You work alongside the Crewmates.
        Independent = 2,  // You work upon your own team.
        LoneWolf = 3      // You work by yourself.
    }

    public enum VentPrivilege
    {
        None = 0,     // You possess an inability to use vents.
        Impostor = 1, // You inheret the venting power of an Impostor.
        [Obsolete("Engineer vents do NOT work at the moment! Please do not attempt to use them! 🤓", true)]
        Engineer = 2  // You inheret the venting power of an Engineer.
    }
}
