using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    // https://stackoverflow.com/questions/7056041/can-you-assign-a-function-to-a-variable-in-c
    public class CustomButton
    {
        public KillButtonCustomData GameInstance; // ADD A NULL CHECK IF YOU USE THIS OK?!
        public string name = "OtherKill";
        public string imageName = "DillyzRoleApi_Rewritten.Assets.kill.png";
        private float _cooldown = 30;
        public float cooldown { 
            get { 
                return _cooldown >= 0 ? _cooldown : GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * Math.Abs(_cooldown); 
            } 
            set {
                _cooldown = value;
            } 
        }
        public bool targetButton = false;
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
        private Func<bool, bool> _canUse; // the bool is the original return

        public static Dictionary<string, CustomButton> buttonMap = new Dictionary<string, CustomButton>();
        public static CustomButton getButtonByName(string name) => buttonMap.ContainsKey(name) ? buttonMap[name] : null;
        public static CustomButton addButton(Assembly epicAssemblyFail, string name, string imageName, float cooldown, bool isTargetButton, string[] allowedRoles, 
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
            DillyzRoleApiMain.Instance.Log.LogInfo($"Button ID {this.globalId} exists under {name}.");

            this.epicAssemblyFail = epicAssemblyFail;
            this.buttonText = this.name = name;
            this.imageName = imageName;
            this._cooldown = cooldown;
            this.targetButton = isTargetButton;
            this.buttonTargetsGhosts = false;
            this.caresAboutMoving = true;
            if (allowedRoles != null)
                this.allowedRoles = allowedRoles.ToList();
            this.canTargetSelf = false;
            if (rolesCantTarget != null)
                this.rolesCantTarget = rolesCantTarget.ToList();

            this.textOutlineColor = this.targetButton ? CustomPalette.KillButtonTextOutline : CustomPalette.PassiveButtonTextOutline;

            this._onClicked = onClicked;
        }

        public bool RoleAllowed(string role) {
            return allowedRoles == null || allowedRoles.Contains(role);
        }

        public void OnClicked(KillButtonCustomData button, bool success) {
            if (this._onClicked != null)
                this._onClicked(button, success);
        }

        public void SetCustomUpdate(Action onUpdate) {
            this._onUpdate = onUpdate;
        }

        // the bool in the return is representing 
        public void SetCanUse(Func<bool, bool> canUse)
        {
            this._canUse = canUse;
        }
        public Action GetCustomUpdate()
        {
            return this._onUpdate;
        }
        public Func<bool, bool> GetCanUse()
        {
            return this._canUse;
        }

        // USE TIMER FUNCS AND VARS!!!!!
        public Action<KillButtonCustomData, bool> useTimerCallback; // button - interupted
        public float useTime = 0f;

        public void SetUseTimeButton(float useTime, Action<KillButtonCustomData, bool> useTimerCallback) {
            this.useTime = Math.Abs(useTime);
            this.useTimerCallback = useTimerCallback;
        }
        // end user time stuff

        public static void ResetAllButtons()
        {
            PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
            if (HudManagerPatch.AllKillButtons != null)
            {
                foreach (KillButtonCustomData button in HudManagerPatch.AllKillButtons)
                {
                    button.lastUse = DateTime.UtcNow;
                    if (button.useTimerMode && button.buttonData.RoleAllowed(DillyzUtil.getRoleName(PlayerControl.LocalPlayer)))
                        button.InterruptUseTimer(true);
                }
            }
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
        public SpriteRenderer blockSpr;
        public bool blockingButton = false;
        public bool showIconOnBlocked = false;

        public bool isSetup = false;

        public bool useTimerMode = false;

        public void Setup(CustomButton buttonData, KillButton killButton)
        {
            this.buttonData = buttonData;
            this.killButton = killButton;
            this.maxCooldown = this.buttonData.cooldown;
            this.lastUse = DateTime.UtcNow;
            this.isSetup = true;

            this.buttonData.GameInstance = this;
        }

        public void Update()
        {
            InternalUpdate();
            buttonData.GetCustomUpdate()?.Invoke();
        }

        private void InternalUpdate()
        {
            if (!isSetup)
                return;

            this.killButton.buttonLabelText.text = this.buttonData.buttonText;
            this.killButton.buttonLabelText.SetOutlineColor(this.buttonData.textOutlineColor);

            if (useTimerMode) {
                TimeSpan useTimeLeft = DateTime.UtcNow - lastUse;
                float useTimeRemaining = this.buttonData.useTime - ((float)useTimeLeft.TotalMilliseconds / 1000f);
                this.killButton.SetCoolDown(useTimeRemaining, this.buttonData.useTime);
                this.killButton.cooldownTimerText.color = Palette.AcceptedGreen;

                if (useTimeRemaining > 0)
                {
                    if (this.buttonData.targetButton)
                        SetTarget(null);
                    this.killButton.SetDisabled();
                    return;
                }
                InterruptUseTimer();
            }

            float timeRemaining = 0;
            if (this.buttonData.cooldown != 0f)
            {
                TimeSpan timeLeft = DateTime.UtcNow - lastUse;
                timeRemaining = maxCooldown - ((float)timeLeft.TotalMilliseconds / 1000f);
                this.killButton.SetCoolDown(timeRemaining < 0f ? 0f : timeRemaining, maxCooldown);
            }
            else
                this.killButton.SetCoolDown(0f, 1f);

            if (blockSpr != null)
            {
                blockSpr.enabled = blockingButton && showIconOnBlocked && !useTimerMode;
                if (blockSpr.enabled)
                    killButton.cooldownTimerText.text = "";
            }

            if (blockingButton) {
                if (this.buttonData.targetButton)
                    SetTarget(null);
                this.killButton.SetDisabled();
                return;
            }

            if (this.buttonData.targetButton)
            {
                SetTarget(DillyzUtil.getClosestPlayer(PlayerControl.LocalPlayer, this.buttonData.rolesCantTarget,
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], !this.buttonData.buttonTargetsGhosts,
                        this.buttonData.canTargetSelf));
                return;
            }

            if (timeRemaining <= 0 && !PlayerControl.LocalPlayer.inVent)
                this.killButton.SetEnabled();
            else
                this.killButton.SetDisabled();
        }

        public bool CanUse()
        {
            if (!isSetup)
                return false;

            bool ogreturn = (MeetingHud.Instance == null && (buttonData.RoleAllowed(DillyzUtil.getRoleName(PlayerControl.LocalPlayer))));
            Func<bool, bool> customret = buttonData.GetCanUse();
            return customret != null ? customret.Invoke(ogreturn) : ogreturn;
        }

        public void SetTarget(PlayerControl player)
        {
            if (!isSetup)
                return;

            if (this.killButton.currentTarget != null && this.killButton.currentTarget != player)
                this.killButton.currentTarget.ToggleHighlight(false, PlayerControl.LocalPlayer.Data.Role.TeamType);

            if (PlayerControl.LocalPlayer.inVent) {
                this.killButton.currentTarget = null;
                this.killButton.SetDisabled();
                return;
            }

            this.killButton.currentTarget = player;
            if (killButton.currentTarget != null)
            {
                this.killButton.currentTarget.ToggleHighlight(true, PlayerControl.LocalPlayer.Data.Role.TeamType);
                this.killButton.SetEnabled();
                return;
            }
            this.killButton.SetDisabled();
        }


        public void InterruptUseTimer() => InterruptUseTimer(false);
        public void InterruptUseTimer(bool actaullyInterrupted) {
            if (!useTimerMode)
                return;

            if (buttonData.useTimerCallback != null)
                buttonData.useTimerCallback(this, actaullyInterrupted);
            useTimerMode = false;
            lastUse = DateTime.UtcNow;
            killButton.cooldownTimerText.color = Palette.White;
        }
    }
}
