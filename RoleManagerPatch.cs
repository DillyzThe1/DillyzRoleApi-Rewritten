using System;
using System.Collections.Generic;
using System.Linq;
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
                DillyzRoleApiMain.Instance.Log.LogInfo("DO I SELECT ROLES NOW?");

                // send reset roles packet
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.ResetRoles, Hazel.SendOption.None, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                CustomRole.roleNameMap.Clear();

                // rng
                Random roleRNG = new Random();

                List<CustomRole> roles = CustomRole.allRoles;
                // assign roles
                for (int r = 0; r < roles.Count; r++)
                {
                    CustomRole role = roles[r];
                    role.curActive = 0;
                    if (role.decoy || !role.roleSeleciton || role.ghostRole)
                        continue;
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

                        if (role.setting_chancePerGame == 0)
                            return;

                        if (role.setting_chancePerGame != 100)
                        {
                            int rolecahcnde = UnityEngine.Random.Range(0, 100);
                            if (role.setting_chancePerGame < rolecahcnde)
                                continue;
                        }

                        role.curActive++;

                        int roleIndex = roleRNG.Next(0, availablePlayers.Count);
                        PlayerControl selectedPlayer = availablePlayers[roleIndex];
                        availablePlayers.Remove(selectedPlayer);
                        DillyzUtil.RpcSetRole(selectedPlayer, role.name);
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
                    pc.RemoveAll(x => !x.Data.IsDead || DillyzUtil.roleSide(x) != CustomRoleSide.Crewmate);
                    int num = pc.Count;
                    IRoleOptionsCollection roleOptions = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions;
                    if (DillyzUtil.InFreeplay())
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
