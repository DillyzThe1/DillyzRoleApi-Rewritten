using System;
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
                DillyzRoleApiMain.Instance.Log.LogInfo("DO I SELECT ROLES NOW?");
                //DillyzRoleApiMain.Instance.Log.LogInfo(PlayerControl.AllPlayerControls);

                // send reset roles packet
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.ResetRoles, Hazel.SendOption.None, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                CustomRole.roleNameMap.Clear();

                // rng
                Random roleRNG = new Random();

                List<CustomRole> roles = CustomRole.allRoles;
                DillyzRoleApiMain.Instance.Log.LogInfo($"Right now, we've got {roles.Count} roles to assign.");
                // assign roles
                for (int r = 0; r < roles.Count; r++)
                {
                    CustomRole role = roles[r];
                    DillyzRoleApiMain.Instance.Log.LogInfo($"{role.name} has nobody in it. (Is it a decoy? {role.decoy}.)");
                    role.curActive = 0;
                    if (role.decoy)
                        continue;
                    DillyzRoleApiMain.Instance.Log.LogInfo($"Let's check out {role.name}.");
                    List<PlayerControl> availablePlayers = PlayerControl.AllPlayerControls.ToArray().ToList();
                    availablePlayers.RemoveAll(x => !DillyzUtil.templateRole(x));

                    if (role.side == CustomRoleSide.Independent || role.side == CustomRoleSide.LoneWolf)
                        availablePlayers.RemoveAll(x => DillyzUtil.roleSide(x) == CustomRoleSide.Impostor);
                    else
                        availablePlayers.RemoveAll(x => DillyzUtil.roleSide(x) != role.side);

                    for (int i = 0; i < role.setting_countPerGame; i++)
                    {
                        if (availablePlayers.Count == 0)
                        {
                            DillyzRoleApiMain.Instance.Log.LogInfo($"Nobody was left to assign ${role.name} to.");
                            continue;
                        }

                        if (role.setting_chancePerGame == 0) {
                            DillyzRoleApiMain.Instance.Log.LogInfo($"{role.name} will NEVER spawn.");
                            return;
                        }

                        if (role.setting_chancePerGame != 100)
                        {
                            int rolecahcnde = UnityEngine.Random.Range(0, 100);
                            DillyzRoleApiMain.Instance.Log.LogInfo($"{role.name} had a {rolecahcnde}% chance this time. It requires {role.setting_chancePerGame}% or LESS.");
                            if (role.setting_chancePerGame < rolecahcnde)
                                continue;
                        }
                        else
                            DillyzRoleApiMain.Instance.Log.LogInfo($"{role.name} will always spawn.");

                        role.curActive++;

                        int roleIndex = roleRNG.Next(0, availablePlayers.Count);
                        PlayerControl selectedPlayer = availablePlayers[roleIndex];
                        availablePlayers.Remove(selectedPlayer);
                        CustomRole.setRoleName(selectedPlayer.PlayerId, role.name);

                        if (role.switchToImpostor && !selectedPlayer.Data.Role.IsImpostor)
                        {
                            selectedPlayer.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                            selectedPlayer.Data.Role = new ImpostorRole();
                        }

                        DillyzRoleApiMain.Instance.Log.LogInfo($"Hey! {selectedPlayer.name} is now the {role.name} of the game!");

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
