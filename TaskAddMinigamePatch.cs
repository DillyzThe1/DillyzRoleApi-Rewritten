using AmongUs.GameOptions;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static DillyzRoleApi_Rewritten.PlayerControlPatch;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(TaskAddButton), nameof(TaskAddButton.Update))]
    class TaskAddButtonPatch_Update
    {
        public static void Postfix(TaskAddButton __instance)
        { 
            if (__instance.Role != null && TaskAddMinigamePatch.customTime)
                __instance.Overlay.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.ShowFolder))]
    class TaskAddMinigamePatch
    {
        public static bool customTime = false;
        public static void Postfix(TaskAdderGame __instance, TaskFolder taskFolder) {
            if (__instance.Hierarchy.Count != 1)
                return;

            List<TaskAddButton> buttons = new List<TaskAddButton>();

            foreach (RoleBehaviour rb in DestroyableSingleton<RoleManager>.Instance.AllRoles)
                the(__instance, rb, buttons);


            Transform trans = __instance.ActiveItems[__instance.ActiveItems.Count - 1];
            float xc = trans.localPosition.x + __instance.fileWidth, yc = trans.localPosition.y, mh = 0f;
            foreach (CustomRole role in CustomRole.allRoles) {
                TaskAddButton taskbutton = UnityEngine.Object.Instantiate<TaskAddButton>(__instance.RoleButton);
                taskbutton.SafePositionWorld = __instance.SafePositionWorld;
                taskbutton.Text.text = $"Be_{role.name}.exe";
                __instance.AddFileAsChild(__instance.Root, taskbutton, ref xc, ref yc, ref mh);
                //taskbutton.Role = null;//DestroyableSingleton<RoleManager>.Instance.AllRoles[0];
                taskbutton.FileImage.color = role.roleColor;
                buttons.Add(taskbutton);

                SpriteRenderer sprrend = taskbutton.Overlay.gameObject.GetComponent<SpriteRenderer>();
                if (taskbutton.Button != null)
                {
                    ControllerManager.Instance.AddSelectableUiElement(taskbutton.Button, false);

                    taskbutton.Button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    taskbutton.Button.OnClick.AddListener((UnityAction)callback);
                    void callback()
                    {
                        if (role.side == CustomRoleSide.Impostor || role.switchToImpostor)
                            PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Impostor);
                        else
                            PlayerControl.LocalPlayer.RpcSetRole(RoleTypes.Crewmate);
                        ShipStatus.Instance.Begin();
                        PlayerControl.LocalPlayer.transform.position = __instance.SafePositionWorld;

                        CustomRole.setRoleName(PlayerControl.LocalPlayer.PlayerId, role.name);

                        foreach (TaskAddButton button in buttons)
                            sprrend.enabled = false;
                        foreach (Transform trans in __instance.ActiveItems)
                        {
                            if (trans.gameObject == null)
                                continue;
                            TaskAddButton taskadd = trans.gameObject.GetComponent<TaskAddButton>();
                            if (taskadd == null || taskadd.Overlay == null)
                                continue;
                            
                            taskadd.Overlay.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        }
                        sprrend.enabled = true;
                        customTime = true;

                        if (role.ghostRole)
                        {
                            PlayerControlPlayer_Die.skipNextAssignment = true;
                            PlayerControl.LocalPlayer.Die(DeathReason.Kill, false);
                        }
                    }

                    taskbutton.RolloverHandler.OutColor = role.roleColor;
                    sprrend.enabled = false;

                    HudManager.Instance.SetHudActive(true);
                }
            } 
        }

        public static void the(TaskAdderGame __instance, RoleBehaviour arbys, List<TaskAddButton> buttons) {
            foreach (Transform trans in __instance.ActiveItems)
            {
                if (trans.gameObject == null)
                    continue;
                TaskAddButton taskadd = trans.gameObject.GetComponent<TaskAddButton>();
                if (taskadd == null)
                    continue;

                if (taskadd.Role != arbys)
                    continue;

                taskadd.Button.OnClick.AddListener((UnityAction)callback);
                void callback() {
                    taskadd.Overlay.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    CustomRole.setRoleName(PlayerControl.LocalPlayer.PlayerId, "");
                    customTime = false;

                    foreach (TaskAddButton waffleironjpeg in buttons)
                        waffleironjpeg.Overlay.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }

                Color32 newColor = CustomPalette.CrewmateBlue;
                switch (arbys.Role)
                {
                    case RoleTypes.Impostor:
                        newColor = CustomPalette.ImpostorRed;
                        break;
                    case RoleTypes.ImpostorGhost:
                        newColor = CustomPalette.ImpostorRed;
                        break;
                    case RoleTypes.Shapeshifter:
                        newColor = CustomPalette.ShapeShifterCrimson;
                        break;
                    case RoleTypes.Engineer:
                        newColor = CustomPalette.EngineerOrange;
                        break;
                    case RoleTypes.Scientist:
                        newColor = CustomPalette.ScientistTeal;
                        break;
                    case RoleTypes.GuardianAngel:
                        newColor = CustomPalette.GuardianAngleLightBlue;
                        break;
                }

                taskadd.RolloverHandler.OutColor = newColor;
                taskadd.FileImage.color = newColor;
                return;
            }
        }
    }
}
