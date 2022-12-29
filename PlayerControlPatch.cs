using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using UnityEngine;
using static DillyzRoleApi_Rewritten.Il2CppItemAttribute;

namespace DillyzRoleApi_Rewritten
{
    class PlayerControlPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class PlayerControlPatch_MurderPlayer
        {
            public static bool Prefix(PlayerControl __instance, PlayerControl target)
            {
                DillyzRoleApiMain.Instance.Log.LogWarning($"{__instance.name} tried to kill {target.name} using the normal PlayerControl.MurderPlayer() function!" +
                                                                                                $" (Did you forget to do DillyzUtil.RpcCommitAssassination()?)");
                DillyzUtil.commitAssassination(__instance, target);
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ToggleHighlight))]
        public class PlayerControlPatch_ToggleHighlight
        {
            public static void Postfix(PlayerControl __instance, bool active, RoleTeamTypes targeterTeam)
            {
                if (active)
                    __instance.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", DillyzUtil.color32ToColor(DillyzUtil.roleColor(PlayerControl.LocalPlayer, true)));
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics._CoSpawnPlayer_d__49), nameof(PlayerPhysics._CoSpawnPlayer_d__49.MoveNext))]
        public class PlayerPhysicsPatch_CoSpawnPlayer
        {
            public static void Postfix(PlayerPhysics._CoSpawnPlayer_d__49 __instance, ref bool __result)
            {
                if (!AmongUsClient.Instance.AmHost || __instance.__4__this.name == "Player(Clone)")
                    return;

                DillyzRoleApiMain.Instance.Log.LogInfo("bruh " + __instance.__4__this.name);

                LobbyConfigManager.UpdateAdvancedRoleValues();
                LobbyConfigManager.UpdateRoleValues();

                // send mods
                List<PluginBuildInfo> allPluginInfos = DillyzRoleApiMain.pluginData;

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.ModCheck, Hazel.SendOption.None, -1);
                writer.Write(allPluginInfos.Count);
                foreach (PluginBuildInfo plugin in allPluginInfos)
                {
                    writer.Write(plugin.Name);
                    writer.Write(plugin.Version);
                    writer.Write(plugin.Id);
                }
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                // send roles
                List<string> allRoles = CustomRole.allRoleNames;

                writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.RoleCheck, Hazel.SendOption.None, -1);
                writer.Write(allRoles.Count);
                foreach (string str in allRoles)
                    writer.Write(str);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnGameStart))]
        public class PlayerControlPlayer_OnGameStart
        {
            public static void Postfix(PlayerControl __instance)
            {
                PlayerControlPlayer_Die.gaurdianAngelAttempts = 0;
                ExileControllerPatch.initial_shifters = DillyzUtil.GetAllOfRole("ShapeShifter").Count;
                ExileControllerPatch.initial_engineers = DillyzUtil.GetAllOfRole("Engineer").Count;
                ExileControllerPatch.initial_scientists = DillyzUtil.GetAllOfRole("Scientist").Count;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
        public class PlayerControlPlayer_Die
        {
            public static bool skipNextAssignment = false;
            public static int gaurdianAngelAttempts = 0;
            public static bool Prefix(PlayerControl __instance, DeathReason reason, bool assignGhostRole)
            {
                if (!DestroyableSingleton<TutorialManager>.InstanceExists && __instance.AmOwner)
                {
                    StatsManager.Instance.LastGameStarted = Il2CppSystem.DateTime.MinValue;
                    StatsManager instance = StatsManager.Instance;
                    float banPoints = instance.BanPoints;
                    instance.BanPoints = banPoints - 1f;
                }
                TempData.LastDeathReason = reason;
                __instance.cosmetics.AnimatePetMourning();
                __instance.Data.IsDead = true;
                __instance.gameObject.layer = LayerMask.NameToLayer("Ghost");
                __instance.cosmetics.SetNameMask(false);
                __instance.cosmetics.PettingHand.StopPetting();

                if (!skipNextAssignment)
                {
                    if (AmongUsClient.Instance.AmHost || DillyzUtil.InFreeplay())
                    {
                        CustomRole playerrole = CustomRole.getByName(DillyzUtil.getRoleName(__instance));
                        if (playerrole != null && playerrole.roletoGhostInto != "")
                        {
                            CustomRole newrole = CustomRole.getByName(playerrole.roletoGhostInto);
                            if (newrole == null || !newrole.ghostRole)
                                DillyzUtil.RpcSetRole(__instance, playerrole.roletoGhostInto);
                        }
                        else
                        {
                            int angelmax = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(AmongUs.GameOptions.RoleTypes.GuardianAngel);
                            int angelchance = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(AmongUs.GameOptions.RoleTypes.GuardianAngel);

                            string targetrole = "";
                            if (DillyzUtil.roleSide(__instance) == CustomRoleSide.Crewmate && (gaurdianAngelAttempts < angelmax || DillyzUtil.InFreeplay()))
                            {
                                int rolecahcnde = UnityEngine.Random.Range(0, 100);
                                if (angelchance != 0 && (angelchance == 100 || angelchance >= rolecahcnde) || DillyzUtil.InFreeplay())
                                    targetrole = "GuardianAngel";
                                gaurdianAngelAttempts++;
                            }
                            DillyzUtil.RpcSetRole(__instance, targetrole);
                        }
                    }
                    else
                        skipNextAssignment = false;
                    GameManager.Instance.OnPlayerDeath(__instance, false);

                    if (__instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);


                    if (__instance.AmOwner)
                    {
                        DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(true);
                        __instance.AdjustLighting();
                    }
                }
                return false;
            }
        }
    }
}