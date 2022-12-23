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
                }
                __result = dist;
            }
            public static bool roleCanVent(PlayerControl player)
            {
                if (DillyzUtil.InFreeplay())
                    return true;

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
        [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
        public class VentOutlinePatch
        {
            public static void Postfix(Vent __instance, bool on, bool mainTarget)
            {
                //__instance.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", on ? 1f: 0f);
                //__instance.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", DillyzUtil.roleColor(PlayerControl.LocalPlayer, false));
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


        [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.CanInteract))]
        public static class ActionButtonInteractionPatch
        {
            public static bool Prefix(ActionButton __instance, ref bool __result)
            {
                CustomRole role = CustomRole.getByName(DillyzUtil.getRoleName(PlayerControl.LocalPlayer));

                if (role == null)
                    return true;

                switch (__instance.name) {
                    case "KillButton":
                        if (role.canKill)
                            __result = true;
                        else
                            __result = false;
                        return false;
                    case "VentButton":
                        if (role.ventPrivilege != VentPrivilege.None)
                            __result = true;
                        else
                            __result = false;
                        return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        [HarmonyPriority(Priority.Last)]
        public static class KillButtonClickPatch
        {
            public static bool Prefix(KillButton __instance)
            {
                /*string rolename = DillyzUtil.getRoleName(PlayerControl.LocalPlayer);
                CustomRole role = CustomRole.getByName(rolename);
                if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.CanMove || __instance.currentTarget == null || __instance.isCoolingDown 
                    || (rolename != "Impostor" && rolename != "ShapeShifter" && role == null))
                    return false;

                if (!role.canKill)
                    return false;

                PlayerControl.LocalPlayer.CheckMurder(__instance.currentTarget);
                __instance.SetTarget(null);*/

                if (__instance.gameObject.GetComponent<KillButtonCustomData>() != null)
                    return false;

                if (DillyzUtil.InFreeplay())
                    return true;

                string rolename = DillyzUtil.getRoleName(PlayerControl.LocalPlayer);
                CustomRole role = CustomRole.getByName(rolename);
                if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.CanMove || __instance.currentTarget == null || __instance.isCoolingDown)
                    return false;

                if (rolename == "Impostor" || rolename == "ShapeShifter" || rolename == "GuardianAngel" || (role != null && role.switchToImpostor))
                    return true;

                if (role == null || !role.canKill)
                    return false;

                HudManagerPatch.lastKillThingForCustoms = DateTime.UtcNow;
                DillyzUtil.RpcCommitAssassination(PlayerControl.LocalPlayer, __instance.currentTarget);
                __instance.SetTarget(null);

                return false;
            }
        }
    }
}
