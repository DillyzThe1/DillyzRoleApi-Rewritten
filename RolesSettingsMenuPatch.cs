﻿using HarmonyLib;
using Hazel;
using Il2CppSystem;
using Il2CppSystem.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static DillyzRoleApi_Rewritten.RoleManagerPatch;
using static UnityEngine.GraphicsBuffer;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Start))]
    public class RolesSettingsMenuPatch_Start
    {
        public static void Postfix(RolesSettingsMenu __instance)
        {
            RolesSettingsMenuPatch.cachedsprites.Clear();
            RolesSettingsMenuPatch_FNFModderReference.customtabs.Clear();

            GameObject ogtab = __instance.AllAdvancedSettingTabs[0].Tab;

            foreach (CustomRole role in CustomRole.allRoles) {
                RolesSettingsMenuPatch.cachedsprites[role.name] = role.settingsSprite;

                GameObject newTab = new GameObject();
                newTab.name = role.name + " Settings";
                newTab.transform.parent = ogtab.transform.parent;
                newTab.transform.position = ogtab.transform.position;
                newTab.transform.localScale = Vector3.one;

                GameObject roleNameCopy = GameObject.Instantiate(ogtab.transform.Find("Role Name").gameObject);
                roleNameCopy.transform.parent = newTab.transform;
                roleNameCopy.transform.localScale = Vector3.one;
                roleNameCopy.transform.position = ogtab.transform.Find("Role Name").position;

                //roleNameCopy.GetComponent<TextMeshPro>().text = role.name;
                TMPTextForcer tmpforce = roleNameCopy.AddComponent<TMPTextForcer>();
                tmpforce.textToDo = role.name;
                tmpforce.tmpToDo = roleNameCopy.GetComponent<TextMeshPro>();

                RolesSettingsMenuPatch_FNFModderReference.customtabs[role.name] = newTab;
            }
        }
    } 

    [Il2CppItem]
    class TMPTextForcer : MonoBehaviour {
        public string textToDo = "";
        public TextMeshPro tmpToDo = null;

        void Update() {
            tmpToDo?.text = textToDo;
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ShowAdvancedSettings))]
    public class RolesSettingsMenuPatch_FNFModderReference
    {
        public static Dictionary<string, GameObject> customtabs = new Dictionary<string, GameObject>();
        public static List<GameObject> AllCustomTabs => customtabs.Values.ToList();
        public static string customRoleToSelect = "Sheriff";
        public static bool Prefix(RolesSettingsMenu __instance, RoleBehaviour role)
        {
            foreach (GameObject obj in AllCustomTabs)
                obj.SetActive(false);

            if (role != null)
                return true;
            foreach (AdvancedRoleSettingsButton arbys in __instance.AllAdvancedSettingTabs)
                arbys.Tab.SetActive(false);

            customtabs[customRoleToSelect]?.SetActive(true);
            customtabs[customRoleToSelect]?.transform.Find("Role Name")?.gameObject.GetComponent<TextMeshPro>()?.SetText(role.name);

            __instance.RoleChancesSettings.SetActive(false);
            __instance.AdvancedRolesSettings.SetActive(true);
            __instance.RefreshChildren();
            return false;
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Start))]
    public class RolesSettingsMenuPatch
    {
        public static Dictionary<string, Sprite> cachedsprites = new Dictionary<string, Sprite>();
        public static void Postfix(RolesSettingsMenu __instance) {
            RoleOptionSetting ogSetting = __instance.AllRoleSettings[0];
            RoleOptionSetting otherSetting = __instance.AllRoleSettings[1];
            RoleOptionSetting lastSetting = __instance.AllRoleSettings[__instance.AllRoleSettings.Count - 1];

            /*ogSetting.transform.Find("Count Plus_TMP").gameObject.GetComponent<PassiveButton>().OnClick.AddListener((UnityEngine.Events.UnityAction)funnny);
            void funnny() {
                HarmonyMain.Instance.Log.LogInfo(ogSetting.transform.Find("Count Plus_TMP").gameObject.GetComponent<TextMeshPro>().color);
            }*/


            int rolesThatUsedY = 0;
            float getNewRoleY() {
                float distBet2 = (otherSetting.transform.position.y - ogSetting.transform.position.y);
                return distBet2 + lastSetting.transform.position.y + (distBet2*rolesThatUsedY);
            }

            foreach (CustomRole role in CustomRole.allRoles)
            {
                GameObject settingParent = new GameObject();
                settingParent.transform.parent = ogSetting.transform.parent;
                settingParent.name = role.name;
                settingParent.layer = ogSetting.gameObject.layer;


                GameObject og_bg = ogSetting.gameObject.transform.Find("Background").gameObject;
                GameObject bg = GameObject.Instantiate(og_bg);
                bg.transform.SetParent(settingParent.transform);
                bg.layer = og_bg.layer;

                PassiveButton bgpb = settingParent.AddComponent<PassiveButton>();
                bgpb.OnDown = bgpb.OnRepeat = false;
                bgpb.OnUp = true;
                bgpb.OnMouseOut = new UnityEngine.Events.UnityEvent();
                bgpb.OnMouseOver = new UnityEngine.Events.UnityEvent();
                bgpb.OnMouseOver.AddListener((UnityEngine.Events.UnityAction)bgover);
                void bgover() {
                    GameSettingMenu.Instance.RoleName.text = role.name; 
                    GameSettingMenu.Instance.RoleBlurb.text = $"The {role.name} is a role created using DillyzRoleAPI v2\n\ngithub.com/DillyzThe1";
                    GameSettingMenu.Instance.RoleIcon.sprite = RolesSettingsMenuPatch.cachedsprites[role.name];
                }
                BoxCollider2D btd5 = settingParent.AddComponent<BoxCollider2D>();
                btd5.size = new Vector2(5.8f, 0.55f);

                TextMeshPro og_TMP = ogSetting.transform.Find("Title_TMP").gameObject.GetComponent<TextMeshPro>();

                TextMeshPro titleText = GameObject.Instantiate(og_TMP);
                titleText.gameObject.transform.SetParent(settingParent.transform);
                titleText.text = role.name;

                #region add texts
                TextMeshPro countPlusText = GameObject.Instantiate(ogSetting.transform.Find("Count Plus_TMP").gameObject.GetComponent<TextMeshPro>());
                countPlusText.gameObject.transform.SetParent(settingParent.transform);
                countPlusText.name = "Count Plus_TMP";
                countPlusText.text = "+";

                TextMeshPro countValueText = GameObject.Instantiate(ogSetting.transform.Find("Count Value_TMP").gameObject.GetComponent<TextMeshPro>());
                countValueText.gameObject.transform.SetParent(settingParent.transform);
                countValueText.name = "Count Value_TMP";
                countValueText.text = role.setting_countPerGame.ToString();

                TextMeshPro countMinusText = GameObject.Instantiate(ogSetting.transform.Find("Count Minus_TMP").gameObject.GetComponent<TextMeshPro>());
                countMinusText.gameObject.transform.SetParent(settingParent.transform);
                countMinusText.name = "Count Minus_TMP";
                countMinusText.text = "-";

                TextMeshPro chancePlusText = GameObject.Instantiate(ogSetting.transform.Find("Chance Plus_TMP").gameObject.GetComponent<TextMeshPro>());
                chancePlusText.gameObject.transform.SetParent(settingParent.transform);
                chancePlusText.name = "Chance Plus_TMP";
                chancePlusText.text = "+";

                TextMeshPro chanceValueText = GameObject.Instantiate(ogSetting.transform.Find("Chance Value_TMP").gameObject.GetComponent<TextMeshPro>());
                chanceValueText.gameObject.transform.SetParent(settingParent.transform);
                chanceValueText.name = "Chance Value_TMP";
                chanceValueText.text = role.setting_chancePerGame.ToString() + "%";

                TextMeshPro chanceMinusText = GameObject.Instantiate(ogSetting.transform.Find("Chance Minus_TMP").gameObject.GetComponent<TextMeshPro>());
                chanceMinusText.gameObject.transform.SetParent(settingParent.transform);
                chanceMinusText.name = "Chance Minus_TMP"; 
                chanceMinusText.text = "-";
                #endregion

                TextMeshPro advancedText = GameObject.Instantiate(ogSetting.transform.Find("More Options").gameObject.GetComponent<TextMeshPro>());
                advancedText.gameObject.transform.SetParent(settingParent.transform);
                advancedText.name = "More Options";
                advancedText.text = "Adv";

                #region add text button functionality
                TextMeshPro[] textsToMakeActive = new TextMeshPro[] { countPlusText, countMinusText, chancePlusText, chanceMinusText, advancedText };
                for (int i = 0; i < textsToMakeActive.Length; i++)
                {
                    TextMeshPro curTMP = textsToMakeActive[i];
                    //HarmonyMain.Instance.Log.LogInfo("prank");
                    PassiveButton tmpButton = curTMP.gameObject.GetComponent<PassiveButton>();

                    //HarmonyMain.Instance.Log.LogInfo("prank23");
                    tmpButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    tmpButton.OnClick.AddListener((UnityEngine.Events.UnityAction)callback);

                    tmpButton.OnMouseOver.AddListener((UnityEngine.Events.UnityAction)bgover);

                    void callback() {
                        //HarmonyMain.Instance.Log.LogInfo(role.name + "'s " + curTMP.name + " was clicked");

                        if (!AmongUsClient.Instance.AmHost)
                            return;

                        switch (curTMP.name) {
                            case "Count Plus_TMP":
                                if (role.setting_countPerGame == 0)
                                    role.setting_chancePerGame = 50;

                                role.setting_countPerGame++;
                                break;
                            case "Count Minus_TMP":
                                role.setting_countPerGame--;

                                if (role.setting_countPerGame == 0)
                                    role.setting_chancePerGame = 0;
                                break;
                            case "Chance Plus_TMP":
                                if (role.setting_countPerGame == 0)
                                    role.setting_countPerGame = 1;

                                role.setting_chancePerGame += 10;
                                break;
                            case "Chance Minus_TMP":
                                role.setting_chancePerGame -= 10;

                                if (role.setting_chancePerGame == 0)
                                    role.setting_countPerGame = 0;
                                break;
                            case "More Options":
                                bgover();
                                RolesSettingsMenuPatch_FNFModderReference.customRoleToSelect = role.name;
                                GameSettingMenu.Instance.ShowAdvancedRoleOptions(null);
                                break;
                        }

                        countValueText.text = role.setting_countPerGame.ToString();
                        chanceValueText.text = role.setting_chancePerGame.ToString() + "%";

                        LobbyConfigManager.Save();

                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSettings, Hazel.SendOption.None, -1);
                        writer.Write((byte)2);
                        // LOBBY_ROLE_SETTING-Jester-Count
                        writer.Write($"LOBBY_ROLE_SETTING-{role.name}-Count");
                        writer.Write(role.setting_countPerGame.ToString());
                        writer.Write($"LOBBY_ROLE_SETTING-{role.name}-Chance");
                        writer.Write(role.setting_chancePerGame.ToString());
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
                #endregion


                settingParent.transform.position = new Vector3(ogSetting.transform.position.x, getNewRoleY(), ogSetting.transform.position.z);

                titleText.transform.localPosition = new Vector3(og_TMP.gameObject.transform.localPosition.x, og_TMP.gameObject.transform.localPosition.y, 0);

                rolesThatUsedY++;
                settingParent.transform.localScale = Vector3.one;
            }
        }
    }
}
