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
            if (HudManagerPatch.AllActiveButtons == null)
                return;
            for (int i = 0; i < HudManagerPatch.AllActiveButtons.Count; i++)
                GameObject.Destroy(HudManagerPatch.AllActiveButtons[i]);
            HudManagerPatch.AllActiveButtons.Clear();
            HudManagerPatch.AllActiveButtons = null;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerPatch
    {
        public static DateTime lastKillThingForCustoms = DateTime.UtcNow;
        public static List<CustomActionButton> AllActiveButtons;

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
            HudManagerPatch.AllActiveButtons = new List<CustomActionButton>();

            foreach (CustomButton button in CustomButton.AllCustomButtons)
            {
                if (true) {
                    Texture2D tex2d = new Texture2D(110, 110);
                    Stream myStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DillyzRoleApi_Rewritten.Assets.uncle_fred.png");
                    myStream.Position = 0;
                    byte[] buttonTexture = new byte[myStream.Length];
                    for (int i = 0; i < myStream.Length;)
                        i += myStream.Read(buttonTexture, i, Convert.ToInt32(myStream.Length) - i);
                    ImageConversion.LoadImage(tex2d, buttonTexture, false);
                    killButton.graphic.sprite = Sprite.Create(tex2d, new Rect(0, 0, 110, 110), Vector2.one * 0.5f, 100f);
                    killButton.buttonLabelText.text = "fred";

                    /*PassiveButton pbjsandwich = killButton.gameObject.GetComponent<PassiveButton>();
                    pbjsandwich.OnClick.RemoveAllListeners();
                    pbjsandwich.OnClick.AddListener((UnityEngine.Events.UnityAction)listener);

                    void listener() {
                        HarmonyMain.Instance.Log.LogInfo("epic clickenining");
                    }*/
                    continue;    
                }

                HarmonyMain.Instance.Log.LogInfo("bruh moment1");
                GameObject buttonObject = new GameObject();
                buttonObject.layer = killButton.gameObject.layer;
                HarmonyMain.Instance.Log.LogInfo("bruh moment2");
                buttonObject.transform.parent = killButton.transform.parent;
                CustomActionButton skillIssue = buttonObject.AddComponent<CustomActionButton>();
                HarmonyMain.Instance.Log.LogInfo("bruh moment3");
                skillIssue.name = button.name + "Button";

                PassiveButton pb = buttonObject.AddComponent<PassiveButton>();
                HarmonyMain.Instance.Log.LogInfo("bruh moment4");
                pb.ClickSound = killButton.gameObject.GetComponent<PassiveButton>().ClickSound;
                pb.OnUp = true;
                pb.OnDown = false;
                pb.OnRepeat = false;
                pb.RepeatDuration = 0.3f;
                pb.TargetActionButton = skillIssue;
                pb.HoldToUse = false;
                pb.hasBeenReleased = true;
                pb.repeatTimer = 0;
                pb.totalHeldTime = 0;
                pb.checkedClickEvent = false;

                HarmonyMain.Instance.Log.LogInfo("bruh moment5");
                BoxCollider2D hitbox = buttonObject.AddComponent<BoxCollider2D>();
                hitbox.size = killButton.GetComponent<BoxCollider2D>().size;
                HarmonyMain.Instance.Log.LogInfo("bruh moment6");
                hitbox.offset = killButton.GetComponent<BoxCollider2D>().offset;


                HarmonyMain.Instance.Log.LogInfo("bruh moment8");
                GameObject buttonContainer = new GameObject();
                buttonContainer.name = "Button";
                buttonContainer.transform.parent = skillIssue.transform;

                // the actual thing lol
                HarmonyMain.Instance.Log.LogInfo("bruh moment7");
                skillIssue.graphic = buttonContainer.AddComponent<SpriteRenderer>();
                skillIssue.graphic.transform.parent = skillIssue.transform;

                HarmonyMain.Instance.Log.LogInfo("bruh moment8");
                skillIssue.usesRemainingSprite = new GameObject().AddComponent<SpriteRenderer>();//GameObject.Instantiate(abilityButton.usesRemainingSprite);
                skillIssue.usesRemainingSprite.transform.parent = skillIssue.transform;
                skillIssue.usesRemainingSprite.name = "Uses";
                skillIssue.usesRemainingSprite.sprite = abilityButton.usesRemainingSprite.sprite;

                HarmonyMain.Instance.Log.LogInfo("bruh moment9");
                skillIssue.usesRemainingText = new GameObject().AddComponent<TextMeshPro>();//skillIssue.usesRemainingSprite.GetComponentInChildren<TextMeshPro>();
                skillIssue.usesRemainingText.transform.parent = skillIssue.usesRemainingSprite.transform;
                skillIssue.usesRemainingText.text = abilityButton.usesRemainingSprite.GetComponentInChildren<TextMeshPro>().text;

                HarmonyMain.Instance.Log.LogInfo("bruh moment10");
                skillIssue.buttonLabelText = new GameObject().AddComponent<TextMeshPro>();//GameObject.Instantiate(killButton.buttonLabelText);
                HarmonyMain.Instance.Log.LogInfo("bruh moment10-2");
                skillIssue.buttonLabelText.transform.parent = buttonContainer.transform;
                
                HarmonyMain.Instance.Log.LogInfo("bruh moment11");
                skillIssue.cooldownTimerText = new GameObject().AddComponent<TextMeshPro>();//GameObject.Instantiate(killButton.cooldownTimerText);
                skillIssue.cooldownTimerText.transform.parent = buttonContainer.transform;

                HarmonyMain.Instance.Log.LogInfo("bruh moment12");
                skillIssue.glyph = new GameObject().AddComponent<ActionMapGlyphDisplay>();//GameObject.Instantiate(killButton.glyph);
                HarmonyMain.Instance.Log.LogInfo("bruh moment12-1");
                skillIssue.glyph.transform.parent = buttonContainer.transform;
                //skillIssue.glyph.sr = skillIssue.glyph.gameObject.AddComponent<SpriteRenderer>();
                //skillIssue.glyph.sr.sprite = killButton.glyph.sr.sprite;

                HarmonyMain.Instance.Log.LogInfo("bruh moment13");
                skillIssue.isCoolingDown = false;
                skillIssue.canInteract = false;
                skillIssue.position = new Vector3();
                skillIssue.Setup(button.globalId);
                skillIssue.transform.parent = buttonParent;
                HudManagerPatch.AllActiveButtons.Add(skillIssue);
                HarmonyMain.Instance.Log.LogInfo("bruh moment14");
            }
            HarmonyMain.Instance.Log.LogInfo("bruh moment123456789D");
        }

        public static void displayActionButton(HudManager __instance, CustomRole localRole, bool udiededed) {
            if (MeetingHud.Instance != null)
            {
                __instance.ImpostorVentButton.gameObject.active = false;
                __instance.KillButton.gameObject.SetActive(false);

                if (AllActiveButtons != null)
                    foreach (CustomActionButton button in AllActiveButtons)
                        button.gameObject.SetActive(false);
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

            if (AllActiveButtons != null && AllActiveButtons.Count > 0)
            {
                foreach (CustomActionButton button in AllActiveButtons)
                    button.gameObject.SetActive(true);//button.CanUse());
            }
            else if (AllActiveButtons == null)
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
