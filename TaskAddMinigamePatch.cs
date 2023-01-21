using AmongUs.GameOptions;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
        public static int pageCount = 0, pageIndex = 0;
        public static void Postfix(TaskAdderGame __instance, TaskFolder taskFolder) {
            if (__instance.Hierarchy.Count != 1)
                return;

            List<TaskAddButton> buttons = new List<TaskAddButton>();

            foreach (RoleBehaviour rb in DestroyableSingleton<RoleManager>.Instance.AllRoles)
                the(__instance, rb, buttons);

            int oldcount = __instance.ActiveItems.Count;
            int curPoint = oldcount - 1; // tracks the cur position
            Transform trans = __instance.ActiveItems[curPoint];
            float xc = trans.localPosition.x + __instance.fileWidth, yc = trans.localPosition.y, mh = 0F;

            DillyzRoleApiMain.Instance.Log.LogInfo("base pos [" + xc + ", " + yc + "]");
            DillyzRoleApiMain.Instance.Log.LogInfo("max pos x " + (__instance.lineWidth));

            foreach (CustomRole role in CustomRole.allRoles) {
                if (role.hiddenFromFreeplay)
                    continue;

                curPoint++;

                TaskAddButton taskbutton = UnityEngine.Object.Instantiate<TaskAddButton>(__instance.RoleButton);
                taskbutton.SafePositionWorld = __instance.SafePositionWorld;
                taskbutton.Text.text = $"Be_{role.name}.exe";

                float smh = 0f;
                smh = Mathf.Max(smh, taskbutton.Text.bounds.size.y + 1.1f);
                if (xc > __instance.lineWidth)
                {
                    xc = 0;
                    yc -= smh;
                    DillyzRoleApiMain.Instance.Log.LogInfo(smh + " days tall");
                    smh = 0f;
                }

                __instance.AddFileAsChild(__instance.Root, taskbutton, ref xc, ref yc, ref mh);
                DillyzRoleApiMain.Instance.Log.LogInfo("new pos [" + xc + ", " + yc + "]");

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
                        if (role.switchToImpostor)
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

            pageCount = Mathf.RoundToInt(Mathf.Ceil(__instance.ActiveItems.Count / 25f));
            DillyzRoleApiMain.Instance.Log.LogInfo("so we have like " + __instance.ActiveItems.Count + " items, which is about " + pageCount + " pages");

            pageIndex = 0;

            if (pageCount > 1)
            {
                StringBuilder stringBuilder = new StringBuilder(64);
                for (int i = 0; i < __instance.Hierarchy.Count; i++)
                {
                    stringBuilder.Append(__instance.Hierarchy[i].FolderName);
                    stringBuilder.Append("\\");
                }
                __instance.PathText.text = stringBuilder.ToString() + " (" + (pageIndex + 1) + "/" + pageCount + ")";
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
