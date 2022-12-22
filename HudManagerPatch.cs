using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TMPro;
using UnityEngine;
using AmongUs.GameOptions;
using InnerNet;
using static Rewired.ButtonLoopSet;
using System.IO;
using System.Reflection;
using Sentry.Internal.Extensions;

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
                    if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && player.Data.Role.IsImpostor)
                        HudManagerPatch.displayColor(__instance, player, (player.Data.RoleType == RoleTypes.Shapeshifter) ?
                                                                        CustomPalette.ShapeShifterCrimson : CustomPalette.ImpostorRed);
                    else if (youre)
                        HudManagerPatch.displayColor(__instance, player, DillyzUtil.roleColor(player, true));
                    else
                        HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
                    continue;
                }

                if (theRole.nameColorPublic || youre || // If the name color was public. || if it's you
                                                        // If it's private but you get it.
                   (theRole.teamCanSeeYou && localRole != null && theRole.name == localRole.name && theRole.side != CustomRoleSide.LoneWolf))
                    HudManagerPatch.displayColor(__instance, player, theRole.roleColor);
                else
                    HudManagerPatch.displayColor(__instance, player, CustomPalette.White);
            }

            if ((AmongUsClient.Instance == null || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
                && AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay)
                return;

            // task list
            bool udiededed = PlayerControl.LocalPlayer.Data.IsDead;
            if (!udiededed || DillyzUtil.getRoleName(PlayerControl.LocalPlayer) == "GuardianAngel")
            {
                string intendedString = DillyzUtil.roleText(PlayerControl.LocalPlayer);
                if (__instance.TaskPanel.taskText.text.Length > 0 && !__instance.TaskPanel.taskText.text.Contains(intendedString))
                {
                    if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Scientist)
                        __instance.TaskPanel.taskText.text =
                            __instance.TaskPanel.taskText.text.Substring(0, __instance.TaskPanel.taskText.text.IndexOf("Scientist Hint") - 1);
                    else if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Engineer)
                        __instance.TaskPanel.taskText.text =
                            __instance.TaskPanel.taskText.text.Substring(0, __instance.TaskPanel.taskText.text.IndexOf("Engineer Hint") - 1);
                    else if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.GuardianAngel)
                        __instance.TaskPanel.taskText.text =
                            __instance.TaskPanel.taskText.text.Substring(0, __instance.TaskPanel.taskText.text.IndexOf("Guardian Angel") - 1);

                    __instance.TaskPanel.taskText.text += intendedString;
                }
            }

            // DOING THIS FOR THE RETURN THING BC I LIKE RETURNS
            displayActionButton(__instance, localRole, udiededed);
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

                Texture2D tex2d = new Texture2D(110, 110);
                Stream myStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(button.imageName);
                if (myStream != null)
                {
                    myStream.Position = 0;
                    byte[] buttonTexture = new byte[myStream.Length];
                    for (int i = 0; i < myStream.Length;)
                        i += myStream.Read(buttonTexture, i, Convert.ToInt32(myStream.Length) - i);
                    ImageConversion.LoadImage(tex2d, buttonTexture, false);
                    newKill.graphic.sprite = Sprite.Create(tex2d, new Rect(0, 0, 110, 110), Vector2.one * 0.5f, 100f);
                }

                PassiveButton pbjsandwich = newKill.gameObject.GetComponent<PassiveButton>();
                pbjsandwich.OnClick.RemoveAllListeners();

                KillButtonCustomData customKillControl = newKill.gameObject.AddComponent<KillButtonCustomData>();
                customKillControl.Setup(button, newKill);

                pbjsandwich.OnClick.AddListener((UnityEngine.Events.UnityAction)listener);

                void listener() {
                    HarmonyMain.Instance.Log.LogInfo("epic clickenining");

                    if (!newKill.isActiveAndEnabled || (newKill.currentTarget == null && customKillControl.buttonData.targetButton) || newKill.isCoolingDown ||
                         PlayerControl.LocalPlayer.Data.IsDead != customKillControl.buttonData.buttonForGhosts || (customKillControl.buttonData.caresAboutMoving 
                                                                                                                && !PlayerControl.LocalPlayer.CanMove))
                    {
                        customKillControl.buttonData.OnClicked(customKillControl, false);
                        return;
                    }

                    customKillControl.lastUse = DateTime.UtcNow;
                    customKillControl.buttonData.OnClicked(customKillControl, true);

                    if (customKillControl.buttonData.targetButton)
                        customKillControl.SetTarget(null);
                }

                AllKillButtons.Add(customKillControl);
            }
            HarmonyMain.Instance.Log.LogInfo("bruh moment123456789D");
        }

        public static void displayActionButton(HudManager __instance, CustomRole localRole, bool udiededed) {
            if (MeetingHud.Instance != null)
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

            if (AllKillButtons != null && AllKillButtons.Count > 0)
            {
                foreach (KillButtonCustomData button in AllKillButtons)
                    button.killButton.gameObject.SetActive(button.CanUse());
            }
            else if (AllKillButtons == null)
                MakeFunnyThing(__instance.KillButton, __instance.AbilityButton);
        }

        public static void displayColor(HudManager __instance, PlayerControl player, Color roleColor) {
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
