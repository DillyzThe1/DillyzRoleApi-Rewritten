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
    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.OnEnable))]
    class TaskAddMinigamePatchStart
    {
        public static GameObject bback;
        public static GameObject bfoward;
        public static void Postfix(TaskAdderGame __instance)
        {
            TaskAddMinigamePatch.lastInstance = __instance;
            DillyzRoleApiMain.Instance.Log.LogInfo("women :coffee:");

            Transform closeTrans = __instance.gameObject.transform.Find("CloseButton");


            __instance.gameObject.transform.Find("TitleText_TMP").gameObject.SetActive(false);

            float dist = -0.575f;

            // DOWN
            bback = GameObject.Instantiate(closeTrans.gameObject);
            bback.name = "ButtonDir_Back";
            bback.transform.parent = closeTrans.parent;
            bback.transform.localPosition += new Vector3(0f, dist, -100f);
            bback.GetComponent<SpriteRenderer>().sprite = DillyzUtil.getSprite(Assembly.GetExecutingAssembly(), "DillyzRoleApi_Rewritten.Assets.arrowbuttonleft.png");

            PassiveButton bback_pb = bback.GetComponent<PassiveButton>();

            if (bback_pb != null)
            {
                ControllerManager.Instance.AddSelectableUiElement(bback_pb, false);
                bback_pb.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                bback_pb.OnClick.AddListener((UnityAction)callback);
                void callback()
                {
                    TaskAddMinigamePatch.pageIndex--;
                    TaskAddMinigamePatch.ReposAllItems();
                }
            }

            // UP
            bfoward = GameObject.Instantiate(closeTrans.gameObject);
            bfoward.name = "ButtonDir_Foward";
            bfoward.transform.parent = closeTrans.parent;
            bfoward.transform.localPosition += new Vector3(0f, dist*2, -100f);
            bfoward.GetComponent<SpriteRenderer>().sprite = DillyzUtil.getSprite(Assembly.GetExecutingAssembly(), "DillyzRoleApi_Rewritten.Assets.arrowbuttonright.png");

            PassiveButton bfoward_pb = bfoward.GetComponent<PassiveButton>();

            if (bback_pb != null)
            {
                ControllerManager.Instance.AddSelectableUiElement(bfoward_pb, false);
                bfoward_pb.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                bfoward_pb.OnClick.AddListener((UnityAction)callback2);
                void callback2()
                {
                    TaskAddMinigamePatch.pageIndex++;
                    TaskAddMinigamePatch.ReposAllItems();
                }
            }
        }
    }
    [HarmonyPatch(typeof(TaskFolder), nameof(TaskFolder.Start))]
    class TaskFolderPatch
    {
        public static void Postfix(TaskFolder __instance) {
            __instance.Button.OnClick.AddListener((UnityAction)TaskAddMinigamePatch.Reset);
        }
    }
    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.ShowFolder))]
    class TaskAddMinigamePatch
    {
        public static bool customTime = false;
        public static int pageCount = 0, pageIndex = 0;
        public static TaskAdderGame lastInstance;

        public static void Postfix(TaskAdderGame __instance, TaskFolder taskFolder)
        {
            TaskAddMinigamePatch.lastInstance = __instance;
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

            foreach (CustomRole role in CustomRole.allRoles)
            {
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
                            PlayerControlPlayer_Die.skipNextAssignment = false;
                        }
                        CustomRole.setRoleName(PlayerControl.LocalPlayer.PlayerId, role.name);
                    }

                    taskbutton.RolloverHandler.OutColor = role.roleColor;
                    sprrend.enabled = false;

                    HudManager.Instance.SetHudActive(true);
                }
            }

            Reset();
        }

        public static void Reset() {
            pageIndex = 0;
            RecalcPages();
            ReposAllItems();
        }

        public static void RecalcPages() {
            pageCount = Mathf.RoundToInt(Mathf.Ceil(lastInstance.ActiveItems.Count / 25f));
            DillyzRoleApiMain.Instance.Log.LogInfo("so we have like " + lastInstance.ActiveItems.Count + " items, which is about " + pageCount + " pages");

            TaskAddMinigamePatchStart.bback.gameObject.SetActive(pageCount > 1);
            TaskAddMinigamePatchStart.bfoward.gameObject.SetActive(pageCount > 1);
        }

        public static void ReposAllItems() {
            if (lastInstance == null)
                return;

            if (TaskAddMinigamePatch.pageIndex < 0)
                TaskAddMinigamePatch.pageIndex = 0;
            if (TaskAddMinigamePatch.pageIndex >= pageCount)
                TaskAddMinigamePatch.pageIndex = pageCount - 1;

            float filedist = 1.2f;

            for (int i = 0; i < lastInstance.ActiveItems.Count; i++) {
                Transform trans = lastInstance.ActiveItems[i];
                trans.localPosition = new Vector3(trans.localPosition.x, ((-filedist) * Mathf.RoundToInt(Mathf.Floor(i / 5f)) + ((filedist * 5f) * ((float)pageIndex))) + 0.55f, trans.localPosition.z);
                trans.gameObject.SetActive(pageIndex == (Mathf.RoundToInt(Mathf.Ceil((i + 1) / 25f)) - 1));
            }

            if (pageCount > 1)
            {
                StringBuilder stringBuilder = new StringBuilder(64);
                for (int i = 0; i < lastInstance.Hierarchy.Count; i++)
                {
                    stringBuilder.Append(lastInstance.Hierarchy[i].FolderName);
                    stringBuilder.Append("\\");
                }
                lastInstance.PathText.text = stringBuilder.ToString() + " (" + (pageIndex + 1) + "/" + pageCount + ")";
            }
        }

        public static void the(TaskAdderGame __instance, RoleBehaviour arbys, List<TaskAddButton> buttons)
        {
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
                void callback()
                {
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
    
    [HarmonyPatch(typeof(TaskAddButton), nameof(TaskAddButton.Update))]
    class TaskAddButtonPatch_Update
    {
        public static void Postfix(TaskAddButton __instance)
        {
            if (__instance.Role != null && TaskAddMinigamePatch.customTime)
                __instance.Overlay.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
