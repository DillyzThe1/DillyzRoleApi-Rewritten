﻿using HarmonyLib;
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

                #region add text button functionality
                TextMeshPro[] textsToMakeActive = new TextMeshPro[] { countPlusText, countMinusText, chancePlusText, chanceMinusText };
                for (int i = 0; i < textsToMakeActive.Length; i++)
                {
                    TextMeshPro curTMP = textsToMakeActive[i];
                    HarmonyMain.Instance.Log.LogInfo("prank");
                    PassiveButton tmpButton = curTMP.gameObject.GetComponent<PassiveButton>();

                    HarmonyMain.Instance.Log.LogInfo("prank23");
                    tmpButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    tmpButton.OnClick.AddListener((UnityEngine.Events.UnityAction)callback);
                    void callback() {
                        HarmonyMain.Instance.Log.LogInfo(role.name + "'s " + curTMP.name + " was clicked");
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