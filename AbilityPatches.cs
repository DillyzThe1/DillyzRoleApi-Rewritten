using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    class AbilityPatches
    {
        [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
        public class VentPatch {
            public static void Postfix(Vent __instance, GameData.PlayerInfo pc, ref bool canUse, ref bool couldUse, ref float __result) {
                float dist = float.MaxValue;
                var obj = pc.Object;
                canUse = couldUse = (obj.CanMove || obj.inVent) && roleCanVent(obj);
                if (canUse)
                {
                    Vector3 truepos = obj.GetTruePosition();
                    Vector3 pos = __instance.transform.position;
                    dist = Vector2.Distance(truepos, pos);
                    canUse &= (dist <= __instance.UsableDistance);

                    __instance.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", DillyzUtil.roleColor(PlayerControl.LocalPlayer, false));
                }
                __result = dist;
            }
            public static bool roleCanVent(PlayerControl player)
            {
                string rolename = DillyzUtil.getRoleName(player);

                if (player.Data.IsDead)
                    return false;

                if (rolename == "Impostor" || rolename == "ShapeShifter" || rolename == "Engineer")
                    return true;

                CustomRole role = CustomRole.getByName(rolename);

                if (role == null)
                    return false;

                return role.ventPrivilege != VentPrivilege.None;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class MurderPatch {
            public static bool Prefix(PlayerControl __instance, PlayerControl target)
            {
                CustomRole role = CustomRole.getByName(DillyzUtil.getRoleName(__instance));
                if (((__instance.Data.RoleType == RoleTypes.Impostor || __instance.Data.RoleType == RoleTypes.Shapeshifter)
                    && !(target.Data.RoleType == RoleTypes.Impostor || target.Data.RoleType == RoleTypes.Shapeshifter))
                    || (role != null && role.switchToImpostor && role.canKill))
                    return true;
                if (role != null && role.canKill)
                {
                    __instance.Data.RoleType = RoleTypes.Impostor;
                    __instance.Data.Role = new ImpostorRole();
                    return true;
                }
                // don't kill em
                return false;
            }
            public static void Postfix(PlayerControl __instance, PlayerControl target)
            {
                CustomRole role = CustomRole.getByName(DillyzUtil.getRoleName(__instance));
                if ((__instance.Data.RoleType == RoleTypes.Impostor || __instance.Data.RoleType == RoleTypes.Shapeshifter)
                                                                    || (role != null && role.switchToImpostor && role.canKill))
                    return;
                if (role != null && role.canKill)
                {
                    __instance.Data.RoleType = RoleTypes.Crewmate;
                    __instance.Data.Role = new CrewmateRole();
                    return;
                }
            }
        }

        // screw ur bans lmao
        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
        public static class AmBannedPatch
        {
            public static void Postfix(out bool __result)
            {
                __result = false;
            }
        }
    }
}
