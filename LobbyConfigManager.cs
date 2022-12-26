
using Hazel;
using Il2CppSystem.Text.Json;
using Sentry.Protocol.Envelopes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    public class LobbyConfigManager
    {
        public static string OUTPUT_FILE => DillyzRoleApiMain.BEPINEX_CONFIG_FOLDER + "dillyzroleapi-lobby-settings.json";
        public static string OUTPUT_FILE_ROLES => DillyzRoleApiMain.BEPINEX_CONFIG_FOLDER + "dillyzroleapi-role-settings.json";

        private static List<LobbyRoleSetting> _lobbyRoleSettings = new List<LobbyRoleSetting>();
        public static List<LobbyRoleSetting> lobbyRoleSettings => _lobbyRoleSettings;

        public static void Save() {
            /*string jsonString = JsonSerializer.Serialize(lobbyRoleSettings, new JsonSerializerOptions() { _writeIndented = true });
            using (StreamWriter outputFile = new StreamWriter(OUTPUT_FILE_ROLES))
                outputFile.WriteLine(jsonString);*/
        }

        public static void Load()
        {
            /*using (StreamReader inputFile = new StreamReader(OUTPUT_FILE_ROLES))
            {
                string json = inputFile.ReadToEnd();
                _lobbyRoleSettings = JsonSerializer.Deserialize<List<LobbyRoleSetting>>(json);
            }*/
        }

        public static void UpdateRoleValues() {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSettings, Hazel.SendOption.None, -1);
            writer.Write((byte)(CustomRole.allRoles.Count * 2));
            foreach (CustomRole role in CustomRole.allRoles)
            {
                writer.Write($"LOBBY_ROLE_SETTING-{role.name}-Count");
                writer.Write(role.setting_countPerGame);
                writer.Write($"LOBBY_ROLE_SETTING-{role.name}-Chance");
                writer.Write(role.setting_chancePerGame);
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void UpdateNormalValues()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSettings, Hazel.SendOption.None, -1);
            writer.Write((byte)CustomRole.allRoles.Count);
            foreach (CustomRole role in CustomRole.allRoles)
            {
                foreach (CustomSetting setting in role.advancedSettings)
                {
                    writer.Write($"LOBBY_ARS-{role.name}-{setting.title}");
                    switch (setting.settingType) {
                        case CustomSettingType.Boolean:
                            writer.Write((setting as CustomBooleanSetting).settingValue);
                            break;
                        case CustomSettingType.Integer:
                            writer.Write((setting as CustomNumberSetting).settingValue);
                            break;
                        case CustomSettingType.String:
                            writer.Write((setting as CustomStringSetting).settingValue);
                            break;
                    }
                }
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    public class LobbyRoleSetting { 
        public string roleName { get; set; }
        public int roleCount { get; set; }
        public int roleChance { get; set; }
    }
}
