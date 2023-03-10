using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Start))]
    public class RolesSettingsMenuPatch_Start
    {
        public static void Postfix(RolesSettingsMenu __instance)
        {
            RolesSettingsMenuPatch.cachedsprites.Clear();
            RolesSettingsMenuPatch_ShowAdvancedSettings.customtabs.Clear();

            GameObject ogtab = null;
            GameObject shapshiftnumopt = null;
            GameObject shapshiftnumopt_2 = null;

            foreach (AdvancedRoleSettingsButton ad in __instance.AllAdvancedSettingTabs)
                if (ad.Tab.name == "GA Settings")
                    ogtab = ad.Tab;
                else if (ad.Tab.name == "Shapeshifter Settings")
                {
                    shapshiftnumopt = ad.Tab.transform.Find("NumberOption").gameObject;
                    shapshiftnumopt_2 = ad.Tab.transform.Find("NumberOption (1)").gameObject;
                }

            GameObject ogBoolOpt = ogtab.transform.Find("ToggleOption").gameObject;
            GameObject ogNumbOpt = ogtab.transform.Find("NumberOption").gameObject;

            Vector3 ogSettingPos = shapshiftnumopt.transform.position;
            float ySpace = shapshiftnumopt_2.transform.position.y - shapshiftnumopt.transform.position.y;
            float ymultlol = 0;

            float xoff = 0.25f;

            foreach (CustomRole role in CustomRole.allRoles)
            {
                if (role.decoy || !role.hasSettings)
                    continue;
                ymultlol = 0f;
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

                TMPTextForce tmpTextForce = roleNameCopy.AddComponent<TMPTextForce>();
                tmpTextForce.forcedText = role.name;
                tmpTextForce.tmp = roleNameCopy.GetComponent<TextMeshPro>();

                RolesSettingsMenuPatch_ShowAdvancedSettings.customtabs[role.name] = newTab;

                DillyzRoleApiMain.Instance.Log.LogInfo("h");
                foreach (CustomSetting setting in role.advancedSettings)
                {
                    GameObject newSettingParent = new GameObject();
                    newSettingParent.gameObject.layer = ogtab.gameObject.layer;
                    newSettingParent.name = setting.title + "Setting";
                    newSettingParent.transform.SetParent(newTab.transform);
                    newSettingParent.transform.position = ogSettingPos + (new Vector3(0, ySpace, 0) * ymultlol);

                    GameObject ogbg = ogBoolOpt.transform.Find("Background").gameObject;
                    GameObject newSettingBG = GameObject.Instantiate(ogbg);
                    newSettingBG.transform.SetParent(newSettingParent.transform);
                    newSettingBG.transform.localPosition = Vector3.zero;
                    newSettingBG.gameObject.layer = ogbg.gameObject.layer;

                    GameObject ogtmp = ogBoolOpt.transform.Find("Title_TMP").gameObject;
                    TextMeshPro titleTMP = GameObject.Instantiate(ogtmp).GetComponent<TextMeshPro>();
                    titleTMP.transform.SetParent(newSettingParent.transform);
                    titleTMP.transform.localPosition = new Vector3(0f, -0.015f, 0f);
                    titleTMP.transform.position = new Vector3(ogtmp.transform.position.x, titleTMP.transform.position.y, -168f);
                    titleTMP.text = setting.title;
                    titleTMP.gameObject.layer = ogtmp.gameObject.layer;

                    switch (setting.settingType) {
                        case CustomSettingType.Boolean:
                            {
                                CustomBooleanSetting boolSetting = setting as CustomBooleanSetting;

                                GameObject ogcheck = ogBoolOpt.transform.Find("CheckBox").gameObject;
                                GameObject checkmarkDupe = GameObject.Instantiate(ogcheck);
                                checkmarkDupe.transform.SetParent(newSettingParent.transform);
                                checkmarkDupe.transform.localPosition = new Vector3(1.65f, 0, 0);
                                checkmarkDupe.gameObject.layer = ogcheck.gameObject.layer;

                                GameObject thecheckmarkitself = checkmarkDupe.transform.Find("CheckMark").gameObject;
                                thecheckmarkitself.gameObject.SetActive(boolSetting.settingValue);
                                thecheckmarkitself.gameObject.layer = ogcheck.gameObject.layer;

                                checkmarkDupe.AddComponent<BoxCollider2D>();
                                PassiveButton pb = checkmarkDupe.AddComponent<PassiveButton>();
                                pb.OnDown = true;
                                pb.OnRepeat = false;
                                pb.OnUp = false;
                                pb.OnMouseOut = new UnityEngine.Events.UnityEvent();
                                pb.OnMouseOut.AddListener((UnityEngine.Events.UnityAction)checkmarkOut);
                                void checkmarkOut()
                                {
                                    checkmarkDupe.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", 0f);
                                    checkmarkDupe.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", Color.clear);
                                }
                                pb.OnMouseOver = new UnityEngine.Events.UnityEvent();
                                pb.OnMouseOver.AddListener((UnityEngine.Events.UnityAction)checkmarkHover);
                                void checkmarkHover()
                                {
                                    foreach (AudioSource audioSource in SoundManager.Instance.allSources.Values)
                                        DillyzRoleApiMain.Instance.Log.LogInfo(audioSource.name + " is playing " + audioSource.clip.name);

                                    checkmarkDupe.GetComponent<SpriteRenderer>().material.SetFloat("_Outline", 1f);
                                    checkmarkDupe.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", DillyzUtil.color32ToColor(CustomPalette.CheckboxSelectedColor));
                                }
                                GameObject checkmarkobj = thecheckmarkitself.gameObject;
                                pb.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                                pb.OnClick.AddListener((UnityEngine.Events.UnityAction)checkmarkClicked);
                                void checkmarkClicked()
                                {
                                    // inverse
                                    boolSetting.settingValue = !boolSetting.settingValue;
                                    checkmarkobj.SetActive(boolSetting.settingValue);

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSettings, Hazel.SendOption.None, -1);
                                    writer.Write((byte)1);
                                    writer.Write($"LOBBY_ARS-{role.name}-{setting.title}");
                                    writer.Write(boolSetting.settingValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                }

                            }
                            break;
                        case CustomSettingType.Float:
                            {
                                CustomFloatSetting floatSetting = setting as CustomFloatSetting;

                                GameObject ogvaluetmp = ogNumbOpt.transform.Find("Value_TMP").gameObject;
                                TextMeshPro valueTMP = GameObject.Instantiate(ogvaluetmp).GetComponent<TextMeshPro>();
                                valueTMP.transform.SetParent(newSettingParent.transform);
                                valueTMP.transform.localPosition = new Vector3(0f, -0.015f, 0f);
                                valueTMP.transform.position = new Vector3(ogvaluetmp.transform.position.x + (xoff/2f), valueTMP.transform.position.y, -168f);
                                valueTMP.gameObject.layer = ogvaluetmp.gameObject.layer;
                                valueTMP.text = floatSetting.settingValue.ToString() + floatSetting.suffix;

                                GameObject ogplustmp = ogNumbOpt.transform.Find("Plus_TMP").gameObject;
                                TextMeshPro plusTMP = GameObject.Instantiate(ogplustmp).GetComponent<TextMeshPro>();
                                plusTMP.transform.SetParent(newSettingParent.transform);
                                plusTMP.gameObject.layer = ogplustmp.gameObject.layer;
                                plusTMP.transform.localPosition = new Vector3(0f, -0.015f, 0f);
                                plusTMP.transform.position = new Vector3(ogplustmp.transform.position.x + xoff, plusTMP.transform.position.y, -168f);

                                PassiveButton plusTMPButton = plusTMP.gameObject.GetComponent<PassiveButton>();
                                plusTMPButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                                plusTMPButton.OnClick.AddListener((UnityEngine.Events.UnityAction)callback_1);
                                void callback_1()
                                {
                                    floatSetting.settingValue += floatSetting.increment;
                                    valueTMP.text = floatSetting.settingValue.ToString() + floatSetting.suffix;

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSettings, Hazel.SendOption.None, -1);
                                    writer.Write((byte)1);
                                    writer.Write($"LOBBY_ARS-{role.name}-{setting.title}");
                                    writer.Write(floatSetting.settingValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                }


                                GameObject ogminustmp = ogNumbOpt.transform.Find("Minus_TMP").gameObject;
                                TextMeshPro minusTMP = GameObject.Instantiate(ogminustmp).GetComponent<TextMeshPro>();
                                minusTMP.transform.SetParent(newSettingParent.transform);
                                minusTMP.gameObject.layer = ogminustmp.gameObject.layer;
                                minusTMP.transform.localPosition = new Vector3(0f, -0.015f, 0f);
                                minusTMP.transform.position = new Vector3(ogminustmp.transform.position.x, minusTMP.transform.position.y, -168f);

                                PassiveButton minusTMPButton = minusTMP.gameObject.GetComponent<PassiveButton>();
                                minusTMPButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                                minusTMPButton.OnClick.AddListener((UnityEngine.Events.UnityAction)callback_2);
                                void callback_2()
                                {
                                    floatSetting.settingValue -= floatSetting.increment;
                                    valueTMP.text = floatSetting.settingValue.ToString() + floatSetting.suffix;

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSettings, Hazel.SendOption.None, -1);
                                    writer.Write((byte)1);
                                    writer.Write($"LOBBY_ARS-{role.name}-{setting.title}");
                                    writer.Write(floatSetting.settingValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                }
                            }
                            break;
                        case CustomSettingType.String:
                            {
                                CustomStringSetting strSetting = setting as CustomStringSetting;

                                GameObject ogvaluetmp = ogNumbOpt.transform.Find("Value_TMP").gameObject;
                                TextMeshPro valueTMP = GameObject.Instantiate(ogvaluetmp).GetComponent<TextMeshPro>();
                                valueTMP.transform.SetParent(newSettingParent.transform);
                                valueTMP.transform.localPosition = new Vector3(0f, -0.015f, 0f);
                                valueTMP.transform.position = new Vector3(ogvaluetmp.transform.position.x + (xoff/2f), valueTMP.transform.position.y, -168f);
                                valueTMP.gameObject.layer = ogvaluetmp.gameObject.layer;
                                valueTMP.text = strSetting.settingValue;

                                GameObject ogplustmp = ogNumbOpt.transform.Find("Plus_TMP").gameObject;
                                TextMeshPro plusTMP = GameObject.Instantiate(ogplustmp).GetComponent<TextMeshPro>();
                                plusTMP.transform.SetParent(newSettingParent.transform);
                                plusTMP.gameObject.layer = ogplustmp.gameObject.layer;
                                plusTMP.transform.localPosition = new Vector3(0f, -0.015f, 0f);
                                plusTMP.transform.position = new Vector3(ogplustmp.transform.position.x + xoff, plusTMP.transform.position.y, -168f);
                                plusTMP.text = ">";

                                PassiveButton plusTMPButton = plusTMP.gameObject.GetComponent<PassiveButton>();
                                plusTMPButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                                plusTMPButton.OnClick.AddListener((UnityEngine.Events.UnityAction)callback_1);
                                void callback_1()
                                {
                                    strSetting.curIndex++;
                                    valueTMP.text = strSetting.settingValue;

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSettings, Hazel.SendOption.None, -1);
                                    writer.Write((byte)1);
                                    writer.Write($"LOBBY_ARS-{role.name}-{setting.title}");
                                    writer.Write(strSetting.settingValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                }


                                GameObject ogminustmp = ogNumbOpt.transform.Find("Minus_TMP").gameObject;
                                TextMeshPro minusTMP = GameObject.Instantiate(ogminustmp).GetComponent<TextMeshPro>();
                                minusTMP.transform.SetParent(newSettingParent.transform);
                                minusTMP.gameObject.layer = ogminustmp.gameObject.layer;
                                minusTMP.transform.localPosition = new Vector3(0f, -0.015f, 0f);
                                minusTMP.transform.position = new Vector3(ogminustmp.transform.position.x, minusTMP.transform.position.y, -168f);
                                minusTMP.text = "<";

                                PassiveButton minusTMPButton = minusTMP.gameObject.GetComponent<PassiveButton>();
                                minusTMPButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                                minusTMPButton.OnClick.AddListener((UnityEngine.Events.UnityAction)callback_2);
                                void callback_2()
                                {
                                    strSetting.curIndex--;
                                    valueTMP.text = strSetting.settingValue;

                                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetSettings, Hazel.SendOption.None, -1);
                                    writer.Write((byte)1);
                                    writer.Write($"LOBBY_ARS-{role.name}-{setting.title}");
                                    writer.Write(strSetting.settingValue);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                }
                            }
                            break;
                    }

                    newSettingParent.transform.localScale = Vector3.one;
                    ymultlol++;
                }
            }
        }
    }

    [Il2CppItem]
    public class TMPTextForce : MonoBehaviour
    {
        public string forcedText = "";
        public TextMeshPro tmp;

        public void Start() { }

        public void Update() {
            if (tmp != null)
                tmp.text = forcedText;
        }
    }

    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.ShowAdvancedSettings))]
    public class RolesSettingsMenuPatch_ShowAdvancedSettings
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

            int rolesThatUsedY = 0;
            float getNewRoleY() {
                float distBet2 = (otherSetting.transform.position.y - ogSetting.transform.position.y);
                return distBet2 + lastSetting.transform.position.y + (distBet2*rolesThatUsedY);
            }

            RolesSettingsMenuPatch_Update.xOg = ogSetting.gameObject.transform.localPosition.x;
            RolesSettingsMenuPatch_Update.yOg = ogSetting.gameObject.transform.localPosition.y;
            RolesSettingsMenuPatch_Update.zOg = ogSetting.gameObject.transform.localPosition.z;
            RolesSettingsMenuPatch_Update.ySpace = otherSetting.gameObject.transform.localPosition.y - ogSetting.gameObject.transform.localPosition.y;

            RolesSettingsMenuPatch_Update.objsToWorryAbout.Clear();

            foreach (RoleOptionSetting setting in __instance.AllRoleSettings)
                RolesSettingsMenuPatch_Update.objsToWorryAbout.Add(setting.gameObject);

            foreach (CustomRole role in CustomRole.allRoles)
            {
                if (!role.hasSettings)
                    continue;

                GameObject settingParent = new GameObject();
                settingParent.transform.parent = ogSetting.transform.parent;
                settingParent.name = role.name;
                settingParent.layer = ogSetting.gameObject.layer;

                RolesSettingsMenuPatch_Update.objsToWorryAbout.Add(settingParent);

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
                    GameSettingMenu.Instance.RoleBlurb.text = role.blurb;
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
                advancedText.enabled = role.advancedSettings.Count > 0;

                #region add text button functionality
                TextMeshPro[] textsToMakeActive = new TextMeshPro[] { countPlusText, countMinusText, chancePlusText, chanceMinusText, advancedText };
                for (int i = 0; i < textsToMakeActive.Length; i++)
                {
                    TextMeshPro curTMP = textsToMakeActive[i];
                    PassiveButton tmpButton = curTMP.gameObject.GetComponent<PassiveButton>();

                    tmpButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    tmpButton.OnClick.AddListener((UnityEngine.Events.UnityAction)callback);

                    tmpButton.OnMouseOver.AddListener((UnityEngine.Events.UnityAction)bgover);

                    void callback() {
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
                                if (role.advancedSettings.Count == 0)
                                    return;

                                bgover();
                                RolesSettingsMenuPatch_ShowAdvancedSettings.customRoleToSelect = role.name;
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
                        writer.Write(role.setting_countPerGame);
                        writer.Write($"LOBBY_ROLE_SETTING-{role.name}-Chance");
                        writer.Write(role.setting_chancePerGame);
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



        [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
        public class RolesSettingsMenuPatch_Update
        {
            public static float xOg = 0f;
            public static float yOg = 0f;
            public static float zOg = 0f;
            public static float ySpace = 0f;
            public static float yOff = 0f;

            public static List<GameObject> objsToWorryAbout = new List<GameObject>();
            public static void Postfix(RolesSettingsMenu __instance) {
                yOff -= Input.mouseScrollDelta.y/6.25f;

                float funny = Math.Abs(Math.Max(objsToWorryAbout.Count - 8, 0) * ySpace);

                if (yOff <= 0)
                    yOff = 0;
                if (objsToWorryAbout.Count < 9)
                    yOff = 0;
                else if (yOff >= funny)
                    yOff = funny;

                for (int i = 0; i < objsToWorryAbout.Count; i++)
                {
                    Transform t = objsToWorryAbout[i].transform;
                    t.localPosition = new Vector3(t.localPosition.x, yOg + ySpace * i + yOff, t.localPosition.z);
                }
            }
        }
    }
}
