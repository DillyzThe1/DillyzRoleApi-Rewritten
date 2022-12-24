﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;

namespace DillyzRoleApi_Rewritten
{
    class RoleManagerPatch
    {

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public class RoleManagerPatch_SelectRoles
        {
            public static void Postfix(RoleManager __instance)
            {
                HarmonyMain.Instance.Log.LogInfo("DO I SELECT ROLES NOW?");
                //HarmonyMain.Instance.Log.LogInfo(PlayerControl.AllPlayerControls);

                // send reset roles packet
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.ResetRoles, Hazel.SendOption.None, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                CustomRole.roleNameMap.Clear();

                // rng
                Random roleRNG = new Random();

                HarmonyMain.Instance.Log.LogInfo($"Right now, we've got {CustomRole.allRoles.Count} roles to assign.");
                // assign roles
                foreach (CustomRole role in CustomRole.allRoles)
                {
                    HarmonyMain.Instance.Log.LogInfo($"Let's check out {role.name}.");
                    List<PlayerControl> availablePlayers = PlayerControl.AllPlayerControls.ToArray().ToList();
                    availablePlayers.RemoveAll(x => !DillyzUtil.templateRole(x));

                    if (role.side == CustomRoleSide.Independent || role.side == CustomRoleSide.LoneWolf)
                        availablePlayers.RemoveAll(x => DillyzUtil.roleSide(x) == CustomRoleSide.Impostor);
                    else
                        availablePlayers.RemoveAll(x => DillyzUtil.roleSide(x) != role.side);

                    for (int i = 0; i < role.setting_countPerGame; i++)
                    {
                        if (availablePlayers.Count == 0)
                            continue;

                        if (role.setting_chancePerGame != 100 && UnityEngine.Random.Range(0, 100) >= role.setting_chancePerGame)
                            return;

                        int roleIndex = roleRNG.Next(0, availablePlayers.Count);
                        PlayerControl selectedPlayer = availablePlayers[roleIndex];
                        availablePlayers.Remove(selectedPlayer);
                        CustomRole.setRoleName(selectedPlayer.PlayerId, role.name);

                        if (role.switchToImpostor && !selectedPlayer.Data.Role.IsImpostor)
                        {
                            selectedPlayer.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                            selectedPlayer.Data.Role = new ImpostorRole();
                        }

                        HarmonyMain.Instance.Log.LogInfo($"Hey! {selectedPlayer.name} is now the {role.name} of the game!");

                        // send role packet
                        writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetRole, Hazel.SendOption.None, -1);
                        writer.Write(selectedPlayer.PlayerId);
                        writer.Write(role.name);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
        }



        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.AssignRoleOnDeath))]
        public class RoleManagerPatch_AssignRoleOnDeath
        {
            public static bool Prefix(RoleManager __instance, PlayerControl player, bool specialRolesAllowed)
            {
                if (player == null || !player.Data.IsDead)
                    return false;

                RoleTypes roleTheyWant = RoleTypes.CrewmateGhost;
                CustomRoleSide rs = DillyzUtil.roleSide(player);

                if (rs == CustomRoleSide.Impostor)
                    roleTheyWant = RoleTypes.ImpostorGhost;
                else if (specialRolesAllowed) 
                {
                    RoleTypes guardRole = RoleTypes.GuardianAngel;
                    List<PlayerControl> pc = PlayerControl.AllPlayerControls.ToArray().ToList();
                    pc.RemoveAll(x => x.Data.IsDead && DillyzUtil.roleSide(x) != CustomRoleSide.Impostor);
                    int num = pc.Count;
                    IRoleOptionsCollection roleOptions = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions;
                    if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
                        roleTheyWant = guardRole;
                    else if (num <= roleOptions.GetNumPerGame(guardRole))
                    {
                        int chancePerGame = roleOptions.GetChancePerGame(guardRole);

                        if (HashRandom.Next(101) < chancePerGame)
                            roleTheyWant = guardRole;
                    }
                }

                player.RpcSetRole(roleTheyWant);

                return false;
            }
        }
    }
}
