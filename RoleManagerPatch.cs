using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Hazel;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    class RoleManagerPatch
    {
        public static void Postfix(RoleManager __instance)
        {
            HarmonyMain.Instance.Log.LogInfo("DO I SELECT ROLES NOW?");
            //HarmonyMain.Instance.Log.LogInfo(PlayerControl.AllPlayerControls);

            // send reset roles packet
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.ResetRoles, Hazel.SendOption.None, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                HarmonyMain.Instance.Log.LogInfo("Taking away " + player.name + "'s role!");
                CustomRole.setRoleName(player.PlayerId, "");
            }

            // rng
            Random roleRNG = new Random();

            HarmonyMain.Instance.Log.LogInfo($"Right now, we've got {CustomRole.allRoles.Count} roles to assign.");
            // assign roles
            foreach (CustomRole role in CustomRole.allRoles)
            {
                HarmonyMain.Instance.Log.LogInfo($"Let's check out {role.name}.");
                List<PlayerControl> availablePlayers = PlayerControl.AllPlayerControls.ToArray().ToList();
                availablePlayers.RemoveAll(x => !DillyzUtil.templateRole(x));

                // TODO: ACTAULLY MAKE THE TEAM REVEAL SCREEN WORK RIGHT!!!!!!!
                if (role.side == CustomRoleSide.Independent || role.side == CustomRoleSide.LoneWolf)
                    availablePlayers.RemoveAll(x => DillyzUtil.roleSide(x) == CustomRoleSide.Impostor);
                else
                    availablePlayers.RemoveAll(x => DillyzUtil.roleSide(x) != role.side);

                // TODO: CHECK FOR AMOUNT OF PEOPLE ADDED AND USE THE CHANCE VIA ROLE SETTINGS!!!!
                // THIS IS THE CHUNK FOR EACH TIME IT LANDS A NEW SPOT!!!

                if (availablePlayers.Count == 0)
                    return;
                int roleIndex = roleRNG.Next(0, availablePlayers.Count);
                PlayerControl selectedPlayer = availablePlayers[roleIndex];
                availablePlayers.Remove(selectedPlayer);
                CustomRole.setRoleName(selectedPlayer.PlayerId, role.name);

                HarmonyMain.Instance.Log.LogInfo($"Hey! {selectedPlayer.name} is now the {role.name} of the game!");

                // send role packet
                writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetRole, Hazel.SendOption.None, -1);
                writer.Write(selectedPlayer.PlayerId);
                writer.Write(role.name);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                // END OF CHUNK

            }
        }
    }
}
