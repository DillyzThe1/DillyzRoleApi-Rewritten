using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DillyzRoleApi_Rewritten
{
    // https://stackoverflow.com/questions/7056041/can-you-assign-a-function-to-a-variable-in-c
    public class CustomButton
    {
        public string name = "OtherKill";
        public string imageName = "DillyzRoleApi_Rewritten.Assets.kill.png";
        private float _cooldown = 30;
        public float cooldown => _cooldown >= 0 ? _cooldown : GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * Math.Abs(_cooldown);
        public bool targetButton = false;
        public bool buttonForGhosts = false;
        public bool buttonTargetsGhosts = false;
        public bool caresAboutMoving = true;
        public List<string> allowedRoles;
        public bool canTargetSelf = false;
        public List<string> rolesCantTarget;

        public string buttonText;
        public Color32 textOutlineColor;

        public byte globalId;
        public byte topGlobalId = 0;

        public Assembly epicAssemblyFail = Assembly.GetExecutingAssembly();

        private Action<KillButtonCustomData, bool> _onClicked;
        private Action _onUpdate;
        private Action<bool> _canUse; // the bool is the original return

        public static Dictionary<string, CustomButton> buttonMap = new Dictionary<string, CustomButton>();
        public static CustomButton getButtonByName(string name) => buttonMap.ContainsKey(name) ? buttonMap[name] : null;
        public static void addButton(Assembly epicAssemblyFail, string name, string imageName, float cooldown, bool isTargetButton, string[] allowedRoles, 
            string[] rolesCantTarget, Action<KillButtonCustomData, bool> onClicked) => 
                            buttonMap[name] = new CustomButton(epicAssemblyFail, name, imageName, cooldown, isTargetButton, allowedRoles, rolesCantTarget, onClicked);
        public static List<CustomButton> AllCustomButtons => buttonMap.Values.ToArray().ToList();

        public static CustomButton getById(byte targetId) {
            foreach (CustomButton button in AllCustomButtons)
                if (button.globalId == targetId)
                    return button;
            return null;
        }

        public CustomButton(Assembly epicAssemblyFail, string name, string imageName, float cooldown, bool isTargetButton, string[] allowedRoles,
                                                        string[] rolesCantTarget, Action<KillButtonCustomData, bool> onClicked)
        {
            this.globalId = topGlobalId++;
            HarmonyMain.Instance.Log.LogInfo($"Button ID {this.globalId} exists under {name}.");

            this.epicAssemblyFail = epicAssemblyFail;
            this.buttonText = this.name = name;
            this.imageName = imageName;
            this._cooldown = cooldown;
            this.targetButton = isTargetButton;
            this.buttonForGhosts = false;
            this.buttonTargetsGhosts = false;
            this.caresAboutMoving = true;
            this.allowedRoles = allowedRoles.ToList();
            this.canTargetSelf = false;
            this.rolesCantTarget = rolesCantTarget.ToList();

            this.textOutlineColor = this.targetButton ? CustomPalette.KillButtonTextOutline : CustomPalette.PassiveButtonTextOutline;

            this._onClicked = onClicked;
        }

        public CustomButton() {
            allowedRoles = new List<string>();
            rolesCantTarget = new List<string>();
        }

        public bool RoleAllowed(string role) {
            return allowedRoles.Contains(role);
        }

        public void OnClicked(KillButtonCustomData button, bool success) {
            if (this._onClicked != null)
                this._onClicked(button, success);
        }

        public void SetCustomUpdate(Action onUpdate) {
            this._onUpdate = onUpdate;
        }

        // the bool in the return is representing 
        public void SetCanUse(Action<bool> canUse)
        {
            this._canUse = canUse;
        }
    }

    // used to attach data to kill button clones
    [Il2CppItem]
    public class KillButtonCustomData : MonoBehaviour
    {
        public float maxCooldown;
        public DateTime lastUse;
        public CustomButton buttonData;
        public KillButton killButton;

        public bool isSetup = false;

        public void Setup(CustomButton buttonData, KillButton killButton)
        {
            this.buttonData = buttonData;
            this.killButton = killButton;
            this.maxCooldown = this.buttonData.cooldown;
            this.lastUse = DateTime.UtcNow;
            this.isSetup = true;
        }

        public void Update()
        {
            if (!isSetup)
                return;

            this.killButton.buttonLabelText.text = this.buttonData.buttonText;
            this.killButton.buttonLabelText.SetOutlineColor(this.buttonData.textOutlineColor);

            TimeSpan timeLeft = DateTime.UtcNow - lastUse;
            int timeRemaining = (int)Math.Ceiling((double)new decimal(maxCooldown - timeLeft.TotalMilliseconds / 1000f));
            this.killButton.SetCoolDown(timeRemaining < 0 ? 0 : timeRemaining, maxCooldown);

            if (this.buttonData.targetButton)
            {
                SetTarget(DillyzUtil.getClosestPlayer(PlayerControl.LocalPlayer, this.buttonData.rolesCantTarget,
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], !this.buttonData.buttonTargetsGhosts,
                        this.buttonData.canTargetSelf));
            }
            else
            {
                if (timeRemaining < 0 || this.buttonData.cooldown == 0)
                    this.killButton.SetEnabled();
                else
                    this.killButton.SetDisabled();
            }
        }

        public bool CanUse()
        {
            if (!isSetup)
                return false;

            return MeetingHud.Instance == null && (buttonData.RoleAllowed(DillyzUtil.getRoleName(PlayerControl.LocalPlayer)) || DillyzUtil.InFreeplay()) 
                                                                                && PlayerControl.LocalPlayer.Data.IsDead == this.buttonData.buttonForGhosts;
        }

        public void SetTarget(PlayerControl player)
        {
            if (!isSetup)
                return;

            if (this.killButton.currentTarget != null && this.killButton.currentTarget != player)
                this.killButton.currentTarget.ToggleHighlight(false, PlayerControl.LocalPlayer.Data.Role.TeamType);
            this.killButton.currentTarget = player;
            if (killButton.currentTarget != null)
            {
                this.killButton.currentTarget.ToggleHighlight(true, PlayerControl.LocalPlayer.Data.Role.TeamType);
                this.killButton.SetEnabled();
                return;
            }
            this.killButton.SetDisabled();
        }
    }
}
