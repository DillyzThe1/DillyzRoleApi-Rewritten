using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using Hazel;
using UnityEngine;

// the idea here is that you'll instiate this class for your own purposes
// example for this:
//     CustomRole.createRole("Jester", "Get voted out to win. You're a real trickster!", true, new Color32(90, 50, 200), false, LoneWolf, false, false);
namespace DillyzRoleApi_Rewritten
{
    public class CustomRole
    {
        public static List<string> allRoleNames {
            get { 
                List<string> strlist = new List<string>();
                foreach (CustomRole role in allRoles)
                    strlist.Add(role.name);
                return strlist;
            }
        }
        public static List<CustomRole> allRoles = new List<CustomRole>();
        public static void appendRole(CustomRole yourRole) => allRoles.Add(yourRole);
        public static CustomRole createRole(string name, string subtext, bool nameColor, bool nameColorPublic, Color32 roleColor, bool canSeeTeam, CustomRoleSide side,
                    VentPrivilege ventPrivilege, bool canKill, bool showEjectText) {
            foreach (CustomRole role in allRoles)
                if (role.name == name)
                {
                    DillyzRoleApiMain.Instance.Log.LogError($"Role by name \"{role.name}\" already exists!");
                    return role;
                }
            CustomRole rolee = new CustomRole(name, subtext, nameColor, nameColorPublic, roleColor, canSeeTeam, side, ventPrivilege, canKill, showEjectText);
            CustomRole.appendRole(rolee);
            return rolee;
        }
        public static CustomRole getByName(string name)
        {
            foreach (CustomRole role in allRoles)
                if (role.name == name)
                    return role;
            return null;
        }

        // player id to role name
        public static Dictionary<byte, string> roleNameMap = new Dictionary<byte, string>();
        public static string getRoleName(byte playerId) => roleNameMap.ContainsKey(playerId) ? roleNameMap[playerId] : "";
        public static void setRoleName(byte playerId, string roleName) {
            PlayerControl player = DillyzUtil.findPlayerControl(playerId);
            string role = DillyzUtil.getRoleName(player);

            if (HudManagerPatch.AllKillButtons != null && playerId == PlayerControl.LocalPlayer.PlayerId)
            {
                player.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
                foreach (KillButtonCustomData button in HudManagerPatch.AllKillButtons)
                {
                    button.lastUse = DateTime.UtcNow;
                    if (button.useTimerMode && button.buttonData.allowedRoles.Contains(role))
                        button.InterruptUseTimer(true);
                }
            }

            /*if (PlayerTask.DestroyTasksOfType<ImportantTextTask>(PlayerControl.LocalPlayer) && !PlayerTask.PlayerHasTaskOfType<NormalPlayerTask>(PlayerControl.LocalPlayer))
                ShipStatus.Instance.Begin();*/
            switch (roleName) {
                case "Impostor":
                    if (player.Data.IsDead) {
                        player.Data.RoleType = RoleTypes.ImpostorGhost;
                        player.Data.Role = new ImpostorGhostRole();
                        player.SetRole(RoleTypes.ImpostorGhost);
                    }
                    else {
                        player.Data.RoleType = RoleTypes.Impostor;
                        player.Data.Role = new ImpostorRole();
                        player.SetRole(RoleTypes.Impostor);
                    }
                    break;
                case "ShapeShifter":
                    if (player.Data.IsDead)
                    {
                        player.Data.RoleType = RoleTypes.ImpostorGhost;
                        player.Data.Role = new ImpostorGhostRole();
                        player.SetRole(RoleTypes.ImpostorGhost);
                    }
                    else
                    {
                        player.Data.RoleType = RoleTypes.Shapeshifter;
                        player.Data.Role = new ShapeshifterRole();
                        player.SetRole(RoleTypes.Shapeshifter);
                    }
                    break;
                case "Scientist":
                    if (player.Data.IsDead)
                    {
                        player.Data.RoleType = RoleTypes.CrewmateGhost;
                        player.Data.Role = new CrewmateGhostRole();
                        player.SetRole(RoleTypes.CrewmateGhost);
                    }
                    else
                    {
                        player.Data.RoleType = RoleTypes.Scientist;
                        player.Data.Role = new ScientistRole();
                        player.SetRole(RoleTypes.Scientist);
                    }
                    break;
                case "Engineer":
                    if (player.Data.IsDead)
                    {
                        player.Data.RoleType = RoleTypes.CrewmateGhost;
                        player.Data.Role = new CrewmateGhostRole();
                        player.SetRole(RoleTypes.CrewmateGhost);
                    }
                    else
                    {
                        player.Data.RoleType = RoleTypes.Engineer;
                        player.Data.Role = new EngineerRole();
                        player.SetRole(RoleTypes.Engineer);
                    }
                    break;
                case "GuardianAngel":
                    if (player.Data.IsDead)
                    {
                        player.Data.RoleType = RoleTypes.GuardianAngel;
                        player.Data.Role = new GuardianAngelRole();
                        player.SetRole(RoleTypes.GuardianAngel);
                    }
                    else {
                        player.Data.RoleType = RoleTypes.Crewmate;
                        player.Data.Role = new CrewmateRole();
                        player.SetRole(RoleTypes.Crewmate);
                    }
                    break;
                case "Crewmate":
                    if (player.Data.IsDead)
                    {
                        player.Data.RoleType = RoleTypes.CrewmateGhost;
                        player.Data.Role = new CrewmateGhostRole();
                        player.SetRole(RoleTypes.CrewmateGhost);
                    }
                    else
                    {
                        player.Data.RoleType = RoleTypes.Crewmate;
                        player.Data.Role = new CrewmateRole();
                        player.SetRole(RoleTypes.Crewmate);
                    }
                    break;
                case "":
                    break;
                default:
                    CustomRole roleee = CustomRole.getByName(roleName);
                    //DillyzRoleApiMain.Instance.Log.LogInfo((roleee == null) + " " + player.name);
                    bool funnyt = (roleee != null && roleee.switchToImpostor);
                    //DillyzRoleApiMain.Instance.Log.LogInfo(funnyt + " that " + player.name + " is an imp");
                    player.SetRole(roleee.ghostRole ? (funnyt ? RoleTypes.ImpostorGhost : RoleTypes.CrewmateGhost)  : (funnyt ? RoleTypes.Impostor : RoleTypes.Crewmate));
                    //player.Data.IsDead = roleee.ghostRole;
                    roleNameMap[playerId] = roleName;

                    //DillyzRoleApiMain.Instance.Log.LogInfo("role " + roleNameMap[playerId]);
                    return;
            }
            roleNameMap[playerId] = "";

            //DillyzRoleApiMain.Instance.Log.LogInfo("roleE " + roleName + " " + roleNameMap[playerId]);
        }

        public List<PlayerControl> AllPlayersWithRole {
            get
            {
                List<PlayerControl> bbbb = PlayerControl.AllPlayerControls.ToArray().ToList();
                bbbb.RemoveAll(x => getRoleName(x.PlayerId) != this.name);
                return bbbb;
            }
        }

        public int curActive = 0; // USED FOR GRAMMAR CHECKS, PLEASE IGNORE THIS!

        public string name = "Role Text";                       // Your role's name.
        public string subtext;                                  // The text that appears under.
        public bool nameColorChanges;                           // Determines if your name color is your role color or just red/white.
        public bool nameColorPublic;                            // Determines if your name color is public to all or not.
        public Color32 roleColor;                               // The current color of your role.
        public bool teamCanSeeYou;                              // Determines if your team can see you.
        public CustomRoleSide side;                             // Determines who you work with.
        //public bool commitsTaxFraud { get; }                  // 192.512.3.62
        public VentPrivilege ventPrivilege;                     // Your vent privelege level.
        public bool canKill;                                    // Are you seriously this blind?
        public string ejectionText;                             // "DillyzThe1 was The Jester"
        public bool switchToImpostor = false;                   // Will switch a crewmate role to an Impostor role.
        public string a_or_an = "an";                           // "DillyzThe1 was a Jester" vs "DillyzThe1 was an Jester"
        public Func<WinConditionState>  returnWinConditionState;// You can return a WinConditionState here. Can set to "delegate() { return WinConditionState.None; }".
        public Func<PlayerControl> rwcsPlayer;                  // A player return for the above just incase.
        private List<CustomSetting> _advancedSettings;          // A list of custom setting data to use.
        [Obsolete("Variable \"decoy\" is obselete! Please use \"roleSelected\" and \"hasSettings\"!")]
         public bool decoy = false;                              // If this role isn't actaully meant for gameplay. Use this if you have no real roles to make.
        public bool roleSeleciton = true;
        public bool hasSettings = true;
        public bool hiddenFromFreeplay = false;

        public string roletoGhostInto = ""; // turns into this role on death
        public bool ghostRole = false; // marks the role as a ghost


        // settings stuff
        private int _countMin = 0, _countMax = 15;
        private int _chanceMin = 0, _chanceMax = 100;
        public int countMin { get { return _countMin; } set { _countMin = Math.Min(Math.Max(value, 0),countMax); } }
        public int countMax { get { return _countMax; } set { _countMax = Math.Min(Math.Max(value, 1), countMin); } }
        public int chanceMin { get { return _chanceMin; } set { _chanceMin = Math.Min(Math.Max(value, 0), chanceMax); } }
        public int chanceMax { get { return _chanceMax; } set { _chanceMax = Math.Min(Math.Max(value, 10), chanceMin); } }

        public List<CustomSetting> advancedSettings => _advancedSettings;

        // LOBBY SETTINGS (GET & SET)
        public int setting_countPerGame { get { return settingsForRole.roleCount; } set { settingsForRole.roleCount = Math.Min(Math.Max(value, countMin), countMax); } }
        public int setting_chancePerGame { get { return settingsForRole.roleChance; } set { settingsForRole.roleChance = Math.Min(Math.Max(value, chanceMin), chanceMax); } }

        private LobbyRoleSetting settingsForRole;
        private Assembly settingsSpriteAssembly = Assembly.GetExecutingAssembly();
        private string settingsSpritePath = "DillyzRoleApi_Rewritten.Assets.settings_genericrole.png";
        public Sprite settingsSprite => DillyzUtil.getSprite(settingsSpriteAssembly, settingsSpritePath);
        public void SetSprite(Assembly assembly, string assetPath) { settingsSpriteAssembly = assembly; settingsSpritePath = assetPath; }

        public CustomRole(string name, string subtext, bool nameColor, bool nameColorPublic, Color32 roleColor, bool canSeeTeam, 
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

            this.switchToImpostor = (this.side == CustomRoleSide.Impostor);

            // vs woudn't let me use turnary operators this time >:/
            if (showEjectText)
            {
                ejectionText += $"The {name}.";
                //ejectionText_bad += $"The <${DillyzUtil.colorToHex(this.roleColor)}>{name}.</color>";
            }
            else
            {
                ejectionText += $"{(side == CustomRoleSide.Impostor ? "" : "not ")}The Impostor.";
                //ejectionText_bad += $"The Impostor.";
            }

            foreach (LobbyRoleSetting setting in LobbyConfigManager.lobbyRoleSettings)
                if (setting.roleName == this.name)
                {
                    settingsForRole = setting;
                    return;
                }

            settingsForRole = new LobbyRoleSetting() { roleName = name, roleCount = 1, roleChance = 50 };
            LobbyConfigManager.lobbyRoleSettings.Add(settingsForRole);
            rwcsPlayer = delegate() { return null; };

            _advancedSettings = new List<CustomSetting>();
        }

        public override string ToString() {
            return $"DillyzRoleApi_Rewritten.CustomRole [name: {this.name}, subtext: {this.subtext}, nameColorChanges: {this.nameColorChanges}, " +
                   $"roleColor: [{this.roleColor.r}, {this.roleColor.g}, {this.roleColor.b}, teamCanSeeYou: {this.teamCanSeeYou}, side: {this.side}, " +
                   $"ventPrivilege: {this.ventPrivilege}, canKill: {this.canKill}]";
        }
         
        public CustomFloatSetting AddAdvancedSetting_Float(string name, float defaultValue, float minimum, float maximum, float increment, Action<float> onChanged) {
            CustomFloatSetting setting = new CustomFloatSetting(name, defaultValue, minimum, maximum, increment, onChanged);
            _advancedSettings.Add(setting);
            return setting;
        }
        public CustomStringSetting AddAdvancedSetting_String(string name, string defaultValue, string[] allValues, Action<string> onChanged) {
            CustomStringSetting setting = new CustomStringSetting(name, defaultValue, allValues.ToList(), onChanged);
            _advancedSettings.Add(setting);
            return setting;
        }
        public CustomBooleanSetting AddAdvancedSetting_Boolean(string name, bool defaultValue, Action<bool> onChanged) {
            CustomBooleanSetting setting = new CustomBooleanSetting(name, defaultValue, onChanged);
            _advancedSettings.Add(setting);
            return setting;
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
            writer.Write(cause != null ? cause.PlayerId : 255);
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

    public enum WinConditionState
    {
        None = 0,       // Nothing happens.
        GameOver = 1,   // You win the game.
        Hold = 2        // You hold off the game. (does not override sabotages)
    }
}
