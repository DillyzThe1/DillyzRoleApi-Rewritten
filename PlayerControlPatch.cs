using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DillyzRoleApi_Rewritten.Il2CppItemAttribute;

namespace DillyzRoleApi_Rewritten
{
    class PlayerControlPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class PlayerControlPatch_MurderPlayer {
            public static bool Prefix(PlayerControl __instance, PlayerControl target) {
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
            public static void Postfix(PlayerControl __instance) {
                ExileControllerPatch.initial_shifters = DillyzUtil.GetAllOfRole("ShapeShifter").Count;
                ExileControllerPatch.initial_engineers = DillyzUtil.GetAllOfRole("Engineer").Count;
                ExileControllerPatch.initial_scientists = DillyzUtil.GetAllOfRole("Scientist").Count;
            }
        }
    }
}
