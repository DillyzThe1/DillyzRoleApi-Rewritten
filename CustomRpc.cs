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
        ResetRoles = 101,   // Takes no arguments. Resets all player roles.
        CustomRoleWin = 102,    // The default jester role's winnning RPC. All jester stuff will move to a standalone mod in the future.
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
                    /*foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        HarmonyMain.Instance.Log.LogInfo("Taking away " + player.name + "'s role!");
                        CustomRole.setRoleName(player.PlayerId, "");
                    }*/
                    CustomRole.roleNameMap.Clear();
                    break;
                case (byte)CustomRpc.CustomRoleWin:
                    GameOverPatch.SetAllToWin(reader.ReadString(), DillyzUtil.findPlayerControl(reader.ReadByte()), false);
                    break;
            }
        }
    }
}
