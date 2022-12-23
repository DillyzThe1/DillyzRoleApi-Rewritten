using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        AvailableSpace = 150    // Available spaces for custom RPC. Register your own with DillyzUtil.regRpcCallback("RpcName", delegate(MessageReader reader) {});
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class CustomRpcHandler {

        private static byte _rpcByteOffset = 0;
        public static byte nextRpcByte => (byte)(((byte)CustomRpc.AvailableSpace) + (_rpcByteOffset++));
        public static bool rpcByteAvailable = ((int)CustomRpc.AvailableSpace + _rpcByteOffset) < 256;

        public static List<Action<MessageReader>> customRpcCallbacks = new List<Action<MessageReader>>();

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
                default:
                    if (callId >= (byte)CustomRpc.AvailableSpace)
                    {
                        
                    }
                    break;
            }
        }
    }
}