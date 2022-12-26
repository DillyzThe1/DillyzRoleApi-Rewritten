using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(TaskAdderGame), nameof(TaskAdderGame.ShowFolder))]
    class TaskAddMinigamePatch
    {
        public static void Postfix(TaskAdderGame __instance, TaskFolder taskFolder) {
            if (__instance.Hierarchy.Count != 1)
                return;

            float xc = 0f, yc = 0f, mh = 0f;
            foreach (CustomRole role in CustomRole.allRoles) {
                TaskAddButton taskbutton = UnityEngine.Object.Instantiate<TaskAddButton>(__instance.RoleButton);
                taskbutton.SafePositionWorld = __instance.SafePositionWorld;
                taskbutton.Text.text = $"Be_{role.name}.exe";
                __instance.AddFileAsChild(__instance.Root, taskbutton, ref xc, ref yc, ref mh);
                taskbutton.Role = DestroyableSingleton<RoleManager>.Instance.AllRoles[0];
                taskbutton.FileImage.color = role.roleColor;
                if (taskbutton.Button != null) {
                    ControllerManager.Instance.AddSelectableUiElement(taskbutton.Button, false);

                    taskbutton.Button.OnClick.RemoveAllListeners();
                    taskbutton.Button.OnClick.AddListener((UnityAction)callback);
                    void callback() {
                        PlayerControl.LocalPlayer.Revive();
                        if (role.side == CustomRoleSide.Impostor || role.switchToImpostor)
                        {
                            PlayerControl.LocalPlayer.Data.RoleType = AmongUs.GameOptions.RoleTypes.Impostor;
                            PlayerControl.LocalPlayer.Data.Role = new ImpostorRole();

                            CustomRole.setRoleName(PlayerControl.LocalPlayer.PlayerId, role.name);
                            return;
                        }
                        PlayerControl.LocalPlayer.Data.RoleType = AmongUs.GameOptions.RoleTypes.Crewmate;
                        PlayerControl.LocalPlayer.Data.Role = new CrewmateRole();

                        CustomRole.setRoleName(PlayerControl.LocalPlayer.PlayerId, role.name);
                    }
                }
            }
        }
    }
}
