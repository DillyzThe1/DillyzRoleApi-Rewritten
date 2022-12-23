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

            int rolesThatUsedY = 0;
            float getNewRoleY() {
                float distBet2 = (otherSetting.transform.position.y - ogSetting.transform.position.y);
                return distBet2 + lastSetting.transform.position.y + (distBet2*rolesThatUsedY);
            }

            foreach (CustomRole role in CustomRole.allRoles) {
                RoleOptionSetting setting = GameObject.Instantiate(ogSetting);

                setting.

                GameObject settingParent = setting.gameObject;
                settingParent.transform.parent = ogSetting.transform.parent;
                setting.TitleText.text = role.name;
                settingParent.name = role.name;
                settingParent.layer = ogSetting.gameObject.layer;
                settingParent.transform.localScale = new Vector3(1f, 1f, 1f);

                TextMeshPro countText = settingParent.transform.Find("Count Value_TMP").gameObject.GetComponent<TextMeshPro>();
                TextMeshPro chanceText = settingParent.transform.Find("Chance Value_TMP").gameObject.GetComponent<TextMeshPro>();
                countText.text = role.setting_countPerGame.ToString();
                chanceText.text = role.setting_countPerGame.ToString();

                // no advanced options yet.
                settingParent.transform.Find("More Options").gameObject.SetActive(false);

                void log(string str) => HarmonyMain.Instance.Log.LogInfo(str);
                bool host = AmongUsClient.Instance.AmHost;

                // objs we will use for placeholders
                GameObject curTextObj;
                PassiveButton curTextButton;

                void replaceFunction(string textobj, UnityEngine.Events.UnityAction callback) {
                    curTextObj = settingParent.transform.Find(textobj).gameObject;
                    curTextButton = curTextObj.GetComponent<PassiveButton>();
                    curTextButton.OnClick.RemoveAllListeners();
                    curTextButton.OnClick.AddListener((UnityEngine.Events.UnityAction)bruhSFX);

                    void bruhSFX() {
                        callback.Invoke();

                        GameSettingMenu.Instance.RoleName.text = role.name;
                        GameSettingMenu.Instance.RoleBlurb.text = $"The {role.name} is a custom role created using DillyzRoleApi v2.<br>{role.subtext}<br><br>github.com/DillyzThe1";
                        //GameSettingMenu.Instance.RoleIcon.sprite = this.Role.Ability.Image;
                    }
                }

                // count
                replaceFunction("Count Minus_TMP", (UnityEngine.Events.UnityAction)countMinus);
                void countMinus() => log(host ? $"Higher count made. ({role.name} count: {countText.text = (--role.setting_countPerGame).ToString()})" : "Denied."); 

                replaceFunction("Count Plus_TMP", (UnityEngine.Events.UnityAction)countPlus);
                void countPlus() => log(host ? $"Lower count made. ({role.name} count: {countText.text = (++role.setting_countPerGame).ToString()})" : "Denied.");

                // chance
                replaceFunction("Count Plus_TMP", (UnityEngine.Events.UnityAction)chanceMinus);
                void chanceMinus() => log(host ? $"Lower chance made. ({role.name} chance: {chanceText.text = (role.setting_chancePerGame += 10).ToString()})" : "Denied.");

                replaceFunction("Chance Plus_TMP", (UnityEngine.Events.UnityAction)chancePlus);
                void chancePlus() => log(host ? $"Higher chance made. ({role.name} chance: {chanceText.text = (role.setting_chancePerGame -= 10).ToString()})" : "Denied.");

                settingParent.transform.position = new Vector3(ogSetting.transform.position.x, getNewRoleY(), ogSetting.transform.position.z);
                rolesThatUsedY++;
            }
        }
    }
}
