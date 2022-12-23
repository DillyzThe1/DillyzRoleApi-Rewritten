using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Start))]
    public class RolesSettingsMenuPatch
    {
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

                TextMeshPro og_TMP = ogSetting.transform.Find("Title_TMP").gameObject.GetComponent<TextMeshPro>();

                TextMeshPro titleText = GameObject.Instantiate(og_TMP);
                titleText.gameObject.transform.SetParent(settingParent.transform);
                titleText.text = role.name;

                #region add texts
                TextMeshPro countPlusText = GameObject.Instantiate(og_TMP);
                countPlusText.gameObject.transform.SetParent(settingParent.transform);
                countPlusText.name = "Count Plus_TMP";
                countPlusText.text = "+";

                TextMeshPro countValueText = GameObject.Instantiate(og_TMP);
                countValueText.gameObject.transform.SetParent(settingParent.transform);
                countValueText.name = "Count Value_TMP";
                countValueText.text = role.setting_countPerGame.ToString();

                TextMeshPro countMinusText = GameObject.Instantiate(og_TMP);
                countMinusText.gameObject.transform.SetParent(settingParent.transform);
                countMinusText.name = "Count Minus_TMP";
                countMinusText.text = "-";

                TextMeshPro chancePlusText = GameObject.Instantiate(og_TMP);
                chancePlusText.gameObject.transform.SetParent(settingParent.transform);
                chancePlusText.name = "Chance Plus_TMP";
                chancePlusText.text = "+";

                TextMeshPro chanceValueText = GameObject.Instantiate(og_TMP);
                chanceValueText.gameObject.transform.SetParent(settingParent.transform);
                chanceValueText.name = "Chance Value_TMP";
                chanceValueText.text = role.setting_chancePerGame.ToString() + "%";

                TextMeshPro chanceMinusText = GameObject.Instantiate(og_TMP);
                chanceMinusText.gameObject.transform.SetParent(settingParent.transform);
                chanceMinusText.name = "Chance Minus_TMP";
                chanceMinusText.text = "-";
                #endregion

                #region add text button functionality
                TextMeshPro[] textsToMakeActive = new TextMeshPro[] { countPlusText, countMinusText, chanceValueText, chanceMinusText };
                for (int i = 0; i < 4; i++)
                {
                    TextMeshPro curTMP = textsToMakeActive[i];
                    HarmonyMain.Instance.Log.LogInfo("prank");
                    PassiveButton tmpButton = curTMP.gameObject.AddComponent<PassiveButton>();
                    tmpButton.enabled = true;
                    tmpButton.OnDown = true;
                    tmpButton.OnUp = tmpButton.OnRepeat = false;

                    HarmonyMain.Instance.Log.LogInfo("prank2");
                    curTMP.gameObject.AddComponent<BoxCollider2D>();

                    HarmonyMain.Instance.Log.LogInfo("prank23");
                    tmpButton.OnClick.RemoveAllListeners();
                    tmpButton.OnClick.AddListener((UnityEngine.Events.UnityAction)callback);
                    void callback() {
                        HarmonyMain.Instance.Log.LogInfo(role.name + "'s " + curTMP.name + " was clicked");
                    }

                    HarmonyMain.Instance.Log.LogInfo("prank234");
                    //tmpButton.OnMouseOver.RemoveAllListeners();
                    tmpButton.OnMouseOver = new UnityEngine.Events.UnityEvent();
                    tmpButton.OnMouseOver.AddListener((UnityEngine.Events.UnityAction)overr);
                    void overr() {
                        curTMP.color = CustomPalette.GameSettingSelectedColor;
                    }
                    HarmonyMain.Instance.Log.LogInfo("prank2354");

                    //tmpButton.OnMouseOut.RemoveAllListeners();
                    tmpButton.OnMouseOut = new UnityEngine.Events.UnityEvent();
                    tmpButton.OnMouseOut.AddListener((UnityEngine.Events.UnityAction)outtt);
                    void outtt()
                    {
                        curTMP.color = CustomPalette.White;
                    }
                }
                #endregion


                settingParent.transform.position = new Vector3(ogSetting.transform.position.x, getNewRoleY(), ogSetting.transform.position.z);

                titleText.transform.localPosition = new Vector3(og_TMP.gameObject.transform.localPosition.x, og_TMP.gameObject.transform.localPosition.y, 0);

                Vector3 posOff = new Vector3(1.75f, 0, 0);
                Vector3 funnyScale = Vector3.one * 1.65f;

                Vector3 funnyScale_value = Vector3.one * 1.2f;

                countPlusText.transform.localScale = funnyScale;
                countPlusText.transform.localPosition = ogSetting.transform.Find("Count Plus_TMP").localPosition + posOff;

                countValueText.transform.localScale = funnyScale_value;
                countValueText.transform.localPosition = ogSetting.transform.Find("Count Value_TMP").localPosition + new Vector3(1.275f, 0, 0);

                countMinusText.transform.localScale = funnyScale;
                countMinusText.transform.localPosition = ogSetting.transform.Find("Count Minus_TMP").localPosition + posOff;

                chancePlusText.transform.localScale = funnyScale;
                chancePlusText.transform.localPosition = ogSetting.transform.Find("Chance Plus_TMP").localPosition + posOff;

                chanceValueText.transform.localScale = funnyScale_value;
                chanceValueText.transform.localPosition = ogSetting.transform.Find("Chance Value_TMP").localPosition + new Vector3(1.075f, 0, 0);

                chanceMinusText.transform.localScale = funnyScale;
                chanceMinusText.transform.localPosition = ogSetting.transform.Find("Chance Minus_TMP").localPosition + posOff;

                rolesThatUsedY++;
                settingParent.transform.localScale = Vector3.one;
            }
        }
    }
}
