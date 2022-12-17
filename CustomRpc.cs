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
        SetRole = 100,      // Takes two arguments: PlayerId (byte) & RoleName (string). Sets the role of the player.
        ResetRoles = 101    // Takes no arguments. Resets all player roles.
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch {
        static void Postfix(byte callId, MessageReader reader) {
            switch (callId) {
                case (byte)CustomRpc.SetRole:
                    byte playerId = reader.ReadByte();
                    string roleName = reader.ReadString();
                    CustomRole.setRoleName(playerId, roleName);
                    break;
                case (byte)CustomRpc.ResetRoles:
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        CustomRole.setRoleName(player.PlayerId, "");
                    break;
            }
        }
    }
}
