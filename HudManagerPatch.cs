using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using AmongUs.GameOptions;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.OnDestroy))]
    class HudManagerDestroyPatch
    {
        public static void Prefix(HudManager __instance)
        {
            if (HudManagerPatch.AllKillButtons == null)
                return;
            for (int i = 0; i < HudManagerPatch.AllKillButtons.Count; i++)
                GameObject.Destroy(HudManagerPatch.AllKillButtons[i]);
            HudManagerPatch.AllKillButtons.Clear();
            HudManagerPatch.AllKillButtons = null;
        }
    }


    [HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive))]
    class HudManagerEnablingPatch
    {
        public static bool isActive = true;

        public static void Postfix(HudManager __instance, bool isActive) {
            HudManagerEnablingPatch.isActive = isActive;
        }
    }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerPatch
    {
        public static DateTime lastKillThingForCustoms = DateTime.UtcNow;
        public static List<KillButtonCustomData> AllKillButtons;

        public static void Postfix(HudManager __instance)
        {
            if (/*AmongUsClient.Instance == null || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started ||*/ PlayerControl.LocalPlayer == null ||
                PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.Role == null)
            {
                //foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                //    HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
                return;
            }

            // colorization
            string rnnnmn = DillyzUtil.getRoleName(PlayerControl.LocalPlayer);
            CustomRole localRole = (rnnnmn != null && rnnnmn != "") ? CustomRole.getByName(rnnnmn) : null;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                bool youre = player.PlayerId == PlayerControl.LocalPlayer.PlayerId;

                string rnnnn = DillyzUtil.getRoleName(player);
                CustomRole theRole = (rnnnn != null && rnnnn != "") ? CustomRole.getByName(rnnnn) : null;

                // if the role is not there or doesn't change color, leave it alone
                if (theRole == null || !theRole.nameColorChanges)
                {
                    if (DillyzUtil.roleSide(PlayerControl.LocalPlayer) == DillyzUtil.roleSide(player))
                        HudManagerPatch.displayColor(__instance, player, DillyzUtil.roleColor(player, true));
                    else if (youre)
                        HudManagerPatch.displayColor(__instance, player, DillyzUtil.roleColor(player, true));
                    else
                        HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
                    continue;
                }

                if (theRole.nameColorPublic || youre || // If the name color was public. || if it's you
                                                        // If it's private but you get it.
                   (theRole.teamCanSeeYou && DillyzUtil.roleSide(player) == DillyzUtil.roleSide(PlayerControl.LocalPlayer) && DillyzUtil.roleSide(player) != CustomRoleSide.LoneWolf))
                    HudManagerPatch.displayColor(__instance, player, theRole.roleColor);
                else
                    HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
            }

            if (AmongUsClient.Instance == null || (!DillyzUtil.InGame() && !DillyzUtil.InFreeplay()))
                return;

            // task list
            bool udiededed = PlayerControl.LocalPlayer.Data.IsDead;
            if (!udiededed || DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "GuardianAngel" || (localRole != null && localRole.ghostRole))
            {
                string intendedString = DillyzUtil.roleText(PlayerControl.LocalPlayer);
                TextMeshPro taskText = __instance.TaskPanel.taskText;
                if (taskText.text.Length > 0 && !taskText.text.Contains(intendedString))
                {
                    if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Scientist)
                        taskText.text = DillyzUtil.SafeSubString(taskText.text, 0, taskText.text.IndexOf("Scientist Hint") - 1);
                    else if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Engineer)
                        taskText.text = DillyzUtil.SafeSubString(taskText.text, 0, taskText.text.IndexOf("Engineer Hint") - 1);
                    else if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.GuardianAngel)
                        taskText.text = DillyzUtil.SafeSubString(taskText.text, 0, taskText.text.IndexOf("Guardian Angel") - 1);

                    taskText.text += intendedString;
                }
            }

            // DOING THIS FOR THE RETURN THING BC I LIKE RETURNS
            displayActionButton(__instance, localRole, udiededed);

            if (AllKillButtons == null)
                MakeFunnyThing(__instance.KillButton, __instance.AbilityButton);
        }

        public static void MakeFunnyThing(KillButton killButton, AbilityButton abilityButton) {
            if (CustomButton.AllCustomButtons.Count < 1)
                return;

            HudManagerPatch.lastKillThingForCustoms = DateTime.UtcNow;

            Transform buttonParent = killButton.gameObject.transform.parent;
            HudManagerPatch.AllKillButtons = new List<KillButtonCustomData>();

            foreach (CustomButton button in CustomButton.AllCustomButtons)
            {
                KillButton newKill = GameObject.Instantiate(killButton);
                newKill.transform.parent = killButton.transform.parent;
                newKill.name = button.name + "CustomButton";

                KillButtonCustomData customKillControl = newKill.gameObject.AddComponent<KillButtonCustomData>();
                customKillControl.Setup(button, newKill);

                Sprite newKillSprite = DillyzUtil.getSprite(customKillControl.buttonData.epicAssemblyFail, button.imageName);
                if (newKillSprite != null)
                    newKill.graphic.sprite = newKillSprite;

                PassiveButton pbjsandwich = newKill.gameObject.GetComponent<PassiveButton>();
                pbjsandwich.OnClick.RemoveAllListeners();


                pbjsandwich.OnClick.AddListener((UnityEngine.Events.UnityAction)listener);

                void listener() {
                    DillyzRoleApiMain.Instance.Log.LogInfo("epic clickenining");

                    if (!newKill.isActiveAndEnabled || (newKill.currentTarget == null && customKillControl.buttonData.targetButton) 
                                                    || newKill.isCoolingDown || (customKillControl.buttonData.caresAboutMoving 
                            && !PlayerControl.LocalPlayer.CanMove) || !newKill.canInteract)
                    {
                        customKillControl.buttonData.OnClicked(customKillControl, false);
                        return;
                    }

                    customKillControl.lastUse = DateTime.UtcNow;
                    customKillControl.buttonData.OnClicked(customKillControl, true);

                    if (customKillControl.buttonData.targetButton)
                        customKillControl.SetTarget(null);

                    customKillControl.useTimerMode = customKillControl.buttonData.useTime > 0f;
                }

                AllKillButtons.Add(customKillControl);
            }
            DillyzRoleApiMain.Instance.Log.LogInfo("bruh moment123456789D");
        }

        public static void displayActionButton(HudManager __instance, CustomRole localRole, bool udiededed) {
            if (MeetingHud.Instance != null || !HudManagerEnablingPatch.isActive)
            {
                __instance.ImpostorVentButton.gameObject.active = false;
                __instance.KillButton.gameObject.SetActive(false);

                if (AllKillButtons != null)
                    foreach (KillButtonCustomData button in AllKillButtons)
                        button.killButton.gameObject.SetActive(false);
                return;
            }

            if (localRole != null)
            {
                __instance.ImpostorVentButton.gameObject.active = (localRole.ventPrivilege == VentPrivilege.Impostor) && !udiededed;
                __instance.KillButton.gameObject.SetActive(localRole.canKill && !udiededed);
                if (__instance.KillButton.gameObject.active)
                {
                    float fullCooldown = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    TimeSpan timeLeft = DateTime.UtcNow - lastKillThingForCustoms;
                    int timeRemaining = (int)Math.Ceiling((double)new decimal(fullCooldown - timeLeft.TotalMilliseconds / 1000f));
                    __instance.KillButton.SetCoolDown(timeRemaining < 0 ? 0 : timeRemaining, fullCooldown);
                    __instance.KillButton.SetTarget(DillyzUtil.getClosestPlayer(PlayerControl.LocalPlayer));
                }
            }
            else {
                string rolename = DillyzUtil.getRoleName(PlayerControl.LocalPlayer);
                bool impostorbuttons = (rolename == "Impostor" || rolename == "ShapeShifter") && !udiededed;
                __instance.ImpostorVentButton.gameObject.active = impostorbuttons;
                __instance.KillButton.gameObject.SetActive(impostorbuttons);
            }

            if (AllKillButtons != null && AllKillButtons.Count > 0)
            {
                foreach (KillButtonCustomData button in AllKillButtons)
                    button.killButton.gameObject.SetActive(button.CanUse());
            }
        }

        public static void displayColor(HudManager __instance, PlayerControl player, Color32 roleColor) {
            string hex = DillyzUtil.colorToHex(roleColor);
            TextMeshPro tmp = player.gameObject.transform.Find("Names").Find("NameText_TMP").GetComponent<TextMeshPro>();
            tmp.text = $"<{hex}>{player.name}</color>";

            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                    if (pva.TargetPlayerId == player.PlayerId) {
                        pva.NameText.text = $"<{hex}>{player.name}</color>";
                        return;
                    }
        }
    }
}
