using System;
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
        CustomRPCCall = 104     // Available spaces for custom RPC. Register your own with DillyzUtil.regRpcCallback("RpcName", delegate(MessageReader reader) {});
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