﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cpp2IL.Core.Analysis.ResultModels;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Text.Json;
using Sentry;

namespace DillyzRoleApi_Rewritten
{
    internal enum CustomRpc
    {
        SetRole = 100,          // Takes two arguments: PlayerId (byte) & RoleName (string). Sets the role of the player.
        ResetRoles = 101,       // Takes no arguments. Resets all player roles.
        CustomRoleWin = 102,    // The default jester role's winnning RPC. All jester stuff will move to a standalone mod in the future.
        Assassinate = 103,      // Custom-made assassination.
        SetSettings = 104,      // Setting the settings sets the setting settings.
        RoleCheck = 105,        // A role check for me to do. If you're missing any roles or have too many, you're kicked out.
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

            DillyzRoleApiMain.Instance.Log.LogError("Callback for RPC \"" + rpcName + "\" not found! (Did you pass a null callback?)" +
                                                "\n    To add a callback, do \"delegate(MessageReader message) {}\" instead of \"null\".");
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class CustomRpcHandler {
        public static List<CustomRpcCallback> customRpcCallbacks = new List<CustomRpcCallback>();

        static void Postfix(byte callId, MessageReader reader) {
            switch (callId) {
                case (byte)CustomRpc.SetRole:
                    if (AmongUsClient.Instance.AmHost)
                        return;

                    byte playerId = reader.ReadByte();
                    string roleName = reader.ReadString();
                    CustomRole.setRoleName(playerId, roleName);
                    break;
                case (byte)CustomRpc.ResetRoles:
                    if (AmongUsClient.Instance.AmHost)
                        return;

                    CustomRole.roleNameMap.Clear();
                    break;
                case (byte)CustomRpc.CustomRoleWin:
                    GameOverPatch.SetAllToWin(reader.ReadString(), DillyzUtil.findPlayerControl(reader.ReadByte()), false);
                    break;
                case (byte)CustomRpc.Assassinate:
                    DillyzUtil.commitAssassination(DillyzUtil.findPlayerControl(reader.ReadByte()), DillyzUtil.findPlayerControl(reader.ReadByte()));
                    break;
                case (byte)CustomRpc.SetSettings:
                    if (AmongUsClient.Instance.AmHost)
                        return;

                    byte settingsCount = reader.ReadByte();

                    for (int i = 0; i < settingsCount; i++)
                    {
                        string settingToSet = reader.ReadString();

                        if (settingToSet.StartsWith("LOBBY_ROLE_SETTING-"))
                        {
                            int epictrtoll = settingToSet.IndexOf("-") + 1;
                            string roleToGet_Wthing = settingToSet.Substring(epictrtoll, settingToSet.Length - epictrtoll);
                            string roleToACTAULLYGet = roleToGet_Wthing.Substring(0, roleToGet_Wthing.LastIndexOf("-"));

                            int thevaluelol = reader.ReadInt32();
                            foreach (LobbyRoleSetting newsetting in LobbyConfigManager.lobbyRoleSettings)
                            {
                                if (newsetting.roleName == roleToACTAULLYGet)
                                {
                                    if (roleToGet_Wthing.EndsWith("-Count"))
                                        newsetting.roleCount = thevaluelol;
                                    else if (roleToGet_Wthing.EndsWith("-Chance"))
                                        newsetting.roleChance = thevaluelol;
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

                            //DillyzRoleApiMain.Instance.Log.LogInfo("funny " + roleToACTAULLYGet + " with " + typeToActaullyGet);

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
                                            //DillyzRoleApiMain.Instance.Log.LogInfo(typeToActaullyGet + " = " + boolsetting.settingValue);
                                            break;
                                        case CustomSettingType.Integer:
                                            CustomNumberSetting intsetting = rolesetting as CustomNumberSetting;
                                            intsetting.settingValue = reader.ReadInt32();
                                            //DillyzRoleApiMain.Instance.Log.LogInfo(typeToActaullyGet + " = " + intsetting.settingValue);
                                            break;
                                        case CustomSettingType.String:
                                            CustomStringSetting strsetting = rolesetting as CustomStringSetting;
                                            strsetting.settingValue = reader.ReadString();
                                            //DillyzRoleApiMain.Instance.Log.LogInfo(typeToActaullyGet + " = " + strsetting.settingValue);
                                            break;
                                    }
                                }
                            }
                            continue;
                        }

                        DillyzRoleApiMain.Instance.Log.LogError($"Bad setting RPC \"{settingToSet}\" found! (Terminating setting RPC)");
                        return;
                    }

                    LobbyConfigManager.Save();
                    break;
                case (byte)CustomRpc.RoleCheck:
                    if (AmongUsClient.Instance.AmHost)
                        return;

                    int rolesToConstruct = reader.ReadInt32();
                    List<string> hostRoles = new List<string>();
                    for (int i = 0; i < rolesToConstruct; i++)
                        hostRoles.Add(reader.ReadString());

                    List<string> clientRoles = CustomRole.allRoleNames;


                    List<string> missingRoles = new List<string>();
                    List<string> extraRoles = new List<string>();

                    /*if (hostRoles.Count != clientRoles.Count) {
                        AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.Custom;
                        AmongUsClient.Instance.LastCustomDisconnect = "Your role's count did <#FF6A00>not match</color> the host's amount.\n(Is a DLL <#FF0000>missing</color>?)";
                        AmongUsClient.Instance.HandleDisconnect(AmongUsClient.Instance.LastDisconnectReason, AmongUsClient.Instance.LastCustomDisconnect);
                        return;
                    }*/

                    foreach (string r in hostRoles)
                        if (!clientRoles.Contains(r))
                            missingRoles.Add(r);
                    foreach (string r in clientRoles)
                        if (!hostRoles.Contains(r))
                            extraRoles.Add(r);

                    bool missedRoles = missingRoles.Count != 0, moreRoles = extraRoles.Count != 0;
                    if (missedRoles || moreRoles) {
                        AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.Custom;
                        AmongUsClient.Instance.LastCustomDisconnect = "Your roles <#FF6A00><b>do not</b></color> match the host's.\n";

                        if (missedRoles)
                        {
                            switch (missingRoles.Count)
                            {
                                case 1:
                                    AmongUsClient.Instance.LastCustomDisconnect += "\n<b><#FF0000>Missing</color> role:</b>\n";
                                    AmongUsClient.Instance.LastCustomDisconnect += $"{missingRoles[0]}.";
                                    break;
                                case 2:
                                    AmongUsClient.Instance.LastCustomDisconnect += "\n<b><#FF0000>Missing</color> roles:</b>\n";
                                    AmongUsClient.Instance.LastCustomDisconnect += $"{missingRoles[0]} & {missingRoles[1]}.";
                                    break;
                                default:
                                    AmongUsClient.Instance.LastCustomDisconnect += "\n<b><#FF0000>Missing</color> roles:</b>\n";
                                    for (int i = 0; i < missingRoles.Count; i++)
                                        AmongUsClient.Instance.LastCustomDisconnect += (i == missingRoles.Count - 1) ? $"& {missingRoles[i]}." : $"{missingRoles[i]}, ";
                                    break;
                            }
                        }
                        if (moreRoles)
                        {
                            switch (extraRoles.Count) {
                                case 1:
                                    AmongUsClient.Instance.LastCustomDisconnect += "\n<b><#6400FF>Extra</color> role:</b>\n";
                                    AmongUsClient.Instance.LastCustomDisconnect += $"{extraRoles[0]}.";
                                    break;
                                case 2:
                                    AmongUsClient.Instance.LastCustomDisconnect += "\n<b><#6400FF>Extra</color> roles:</b>\n";
                                    AmongUsClient.Instance.LastCustomDisconnect += $"{extraRoles[0]} & {extraRoles[1]}.";
                                    break;
                                default:
                                    AmongUsClient.Instance.LastCustomDisconnect += "\n<b><#6400FF>Extra</color> roles:</b>\n";
                                    for (int i = 0; i < extraRoles.Count; i++)
                                        AmongUsClient.Instance.LastCustomDisconnect += (i == extraRoles.Count - 1) ? $"& {extraRoles[i]}." : $"{extraRoles[i]}, ";
                                    break;
                            }
                        }

                        AmongUsClient.Instance.HandleDisconnect(AmongUsClient.Instance.LastDisconnectReason, AmongUsClient.Instance.LastCustomDisconnect);
                        return;
                    }

                    break;
                case (byte)CustomRpc.CustomRPCCall:
                    string rpcToGet = reader.ReadString();

                    foreach (CustomRpcCallback callback in customRpcCallbacks)
                        if (callback.rpcName == rpcToGet)
                        {
                            callback.InvokeCallback(reader);
                            return;
                        }

                    DillyzRoleApiMain.Instance.Log.LogError("Warning! No rpc called " + rpcToGet + " exists! Why did you even call it?!");
                    break;
            }
        }
    }
}