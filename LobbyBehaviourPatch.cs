using AmongUs.GameOptions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
    class LobbyBehaviourPatch
    {
        public static void Postfix(LobbyBehaviour __instance) {
            if (GameOptionsManager.Instance.CurrentGameOptions != null)
            {
                TextMeshPro gs = DestroyableSingleton<HudManager>.Instance.GameSettings;
                // cut off the roles settings
                if (gs.text.Contains("Scientist:"))
                    gs.text = gs.text.Substring(0, gs.text.IndexOf("Scientist:"));

                Transform gstextalttrans = gs.gameObject.transform.parent.Find("gstextalt");
                TextMeshPro gstextalt = null;
                if (gstextalttrans == null) {
                    gstextalt = GameObject.Instantiate(gs);
                    gstextalt.transform.parent = gs.transform.parent;
                    gstextalt.name = "gstextalt";
                    gstextalt.alignment = TextAlignmentOptions.Right;
                } 
                else
                    gstextalt = gstextalttrans.gameObject.GetComponent<TextMeshPro>();

                gstextalt.gameObject.SetActive(gs.isActiveAndEnabled);
                gstextalt.transform.position = new Vector3(HudManager.Instance.transform.Find("Buttons").Find("TopRight").Find("MenuButton").transform.position.x - 2.75f, gs.transform.position.y + 0.8f, gs.transform.position.z);
                //gs.transform.position + new Vector3(4.8f, 0.8f, 0f);

                gstextalt.text = "# Role Settings\n";
                foreach (RoleBehaviour roleBehaviour in DestroyableSingleton<RoleManager>.Instance.AllRoles)
                {
                    if (roleBehaviour.Role != RoleTypes.Crewmate && roleBehaviour.Role != RoleTypes.Impostor && roleBehaviour.Role != RoleTypes.CrewmateGhost && roleBehaviour.Role != RoleTypes.ImpostorGhost)
                    {
                        string name = DestroyableSingleton<TranslationController>.Instance.GetString(roleBehaviour.StringName, new Il2CppSystem.Object[0]);
                        int count = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(roleBehaviour.Role);
                        int chance = GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(roleBehaviour.Role);
                        gstextalt.text += $"{name}: {count} with {chance}% Chance\n";
                    }
                }
                foreach (CustomRole role in CustomRole.allRoles) {
                    gstextalt.text += $"{role.name}: {role.setting_countPerGame} with {role.setting_chancePerGame}% Chance\n";
                }
            }
        }
    }
}
