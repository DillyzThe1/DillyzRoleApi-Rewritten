﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cpp2IL.Core.Analysis.ResultModels;
using HarmonyLib;
using Hazel;

namespace DillyzRoleApi_Rewritten
{
    internal enum CustomRpc
    {
        SetRole = 100,          // Takes two arguments: PlayerId (byte) & RoleName (string). Sets the role of the player.
        ResetRoles = 101,       // Takes no arguments. Resets all player roles.
        CustomRoleWin = 102,    // The default jester role's winnning RPC. All jester stuff will move to a standalone mod in the future.
        Assassinate = 103,      // Custom-made assassination.
        SetSettings = 104,      // Setting the settings sets the setting settings.
        CustomRPCCall = 110     // Available spaces for custom RPC. Register your own with DillyzUtil.regRpcCallback("RpcName", delegate(MessageReader reader) {});
    }

    class CustomRpcCallback {
        public string rpcName;
        private Action<MessageReader> _callback;

        public CustomRpcCallback(string rpcName, Action<MessageReader> callback)
        {
            this.rpcName = rpcName;
            this._callback = callback;
        }

        public void InvokeCallback(MessageReader reader) {
            if (this._callback != null)
            {
                this._callback(reader);
                return;
            }

            HarmonyMain.Instance.Log.LogError("Callback for RPC \"" + rpcName + "\" not found! (Did you pass a null callback?)" +
                                                "\n    To add a callback, do \"delegate(MessageReader message) {}\" instead of \"null\".");
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class CustomRpcHandler {
        public static List<CustomRpcCallback> customRpcCallbacks = new List<CustomRpcCallback>();

        static void Postfix(byte callId, MessageReader reader) {
            switch (callId) {
                case (byte)CustomRpc.SetRole:
                    byte playerId = reader.ReadByte();
                    string roleName = reader.ReadString();
                    CustomRole.setRoleName(playerId, roleName);
                    break;
                case (byte)CustomRpc.ResetRoles:
                    CustomRole.roleNameMap.Clear();
                    break;
                case (byte)CustomRpc.CustomRoleWin:
                    GameOverPatch.SetAllToWin(reader.ReadString(), DillyzUtil.findPlayerControl(reader.ReadByte()), false);
                    break;
                case (byte)CustomRpc.Assassinate:
                    DillyzUtil.commitAssassination(DillyzUtil.findPlayerControl(reader.ReadByte()), DillyzUtil.findPlayerControl(reader.ReadByte()));
                    break;
                case (byte)CustomRpc.SetSettings:
                    byte settingsCount = reader.ReadByte();

                    for (int i = 0; i < settingsCount; i++)
                    {
                        string settingToSet = reader.ReadString();

                        if (settingToSet.StartsWith("LOBBY_ROLE_SETTING-"))
                        {
                            int epictrtoll = settingToSet.IndexOf("-") + 1;
                            string roleToGet_Wthing = settingToSet.Substring(epictrtoll, settingToSet.Length - epictrtoll);
                            string roleToACTAULLYGet = roleToGet_Wthing.Substring(0, roleToGet_Wthing.LastIndexOf("-"));

                            string thevaluelol = reader.ReadString();
                            foreach (LobbyRoleSetting newsetting in LobbyConfigManager.lobbyRoleSettings)
                            {
                                if (newsetting.roleName == roleToACTAULLYGet)
                                {
                                    if (roleToGet_Wthing.EndsWith("-Count"))
                                        newsetting.roleCount = Int32.Parse(thevaluelol);
                                    else if (roleToGet_Wthing.EndsWith("-Chance"))
                                        newsetting.roleChance = Int32.Parse(thevaluelol);
                                }
                            }
                            continue;
                        }

                        // LOBBY_ARS-Sheriff-Punished
                        if (settingToSet.StartsWith("LOBBY_ARS-"))
                        {
                            int epictrtoll = settingToSet.IndexOf("-") + 1;
                            string roleToGet_Wthing = settingToSet.Substring(epictrtoll, settingToSet.Length - epictrtoll);
                            string roleToACTAULLYGet = roleToGet_Wthing.Substring(0, roleToGet_Wthing.LastIndexOf("-"));
                            int welcomeold = roleToGet_Wthing.LastIndexOf("-") + 1;
                            string typeToActaullyGet = roleToGet_Wthing.Substring(welcomeold, roleToGet_Wthing.Length - welcomeold);

                            CustomRole role = CustomRole.getByName(roleToACTAULLYGet);

                            //HarmonyMain.Instance.Log.LogInfo("funny " + roleToACTAULLYGet + " with " + typeToActaullyGet);

                            if (role == null)
                                continue;

                            foreach (CustomSetting rolesetting in role.advancedSettings)
                            {
                                if (rolesetting.title == typeToActaullyGet)
                                {
                                    switch (rolesetting.settingType)
                                    {
                                        case CustomSettingType.Boolean:
                                            CustomBooleanSetting boolsetting = rolesetting as CustomBooleanSetting;
                                            boolsetting.settingValue = reader.ReadBoolean();
                                            HarmonyMain.Instance.Log.LogInfo(typeToActaullyGet + " = " + boolsetting.settingValue);
                                            break;
                                        case CustomSettingType.Integer:
                                            CustomNumberSetting intsetting = rolesetting as CustomNumberSetting;
                                            intsetting.settingValue = reader.ReadInt32();
                                            HarmonyMain.Instance.Log.LogInfo(typeToActaullyGet + " = " + intsetting.settingValue);
                                            break;
                                        case CustomSettingType.String:
                                            CustomStringSetting strsetting = rolesetting as CustomStringSetting;
                                            strsetting.settingValue = reader.ReadString();
                                            HarmonyMain.Instance.Log.LogInfo(typeToActaullyGet + " = " + strsetting.settingValue);
                                            break;
                                    }
                                }
                            }
                            continue;
                        }

                        HarmonyMain.Instance.Log.LogError("bad rpc " + settingToSet);
                    }

                    LobbyConfigManager.Save();
                    break;
                case (byte)CustomRpc.CustomRPCCall:
                    string rpcToGet = reader.ReadString();

                    foreach (CustomRpcCallback callback in customRpcCallbacks)
                        if (callback.rpcName == rpcToGet)
                        {
                            callback.InvokeCallback(reader);
                            return;
                        }

                    HarmonyMain.Instance.Log.LogError("Warning! No rpc called " + rpcToGet + " exists! Why did you even call it?!");
                    break;
            }
        }
    }
}