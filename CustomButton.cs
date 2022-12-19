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
    class CustomButton
    {
        public string name = "OtherKill";
        public string imageName = "DillyzRoleApi-Rewritten.Assets.kill.png";
        public float cooldown = 30;
        public bool targetButton = false;
        public bool buttonForGhosts = false;
        public bool buttonTargetsGhosts = false;
        public bool caresAboutMoving = true;
        public List<string> allowedRoles;
        public bool canTargetSelf = false;
        public List<string> rolesCantTarget;

        public Action<CustomActionButton, bool> onClicked;

        public static Dictionary<string, CustomButton> buttonMap = new Dictionary<string, CustomButton>();
        public static CustomButton getButtonByName(string name) => buttonMap.ContainsKey(name) ? buttonMap[name] : null;
        public static void addButton(string name, string imageName, float cooldown, bool isTargetButton, string[] allowedRoles, string[] rolesCantTarget, 
                        Action<CustomActionButton, bool> onClicked) => buttonMap[name] = new CustomButton(name, imageName, cooldown, isTargetButton, allowedRoles, 
                                                                                                                                        rolesCantTarget, onClicked);
        public static List<CustomButton> AllCustomButtons => buttonMap.Values.ToArray().ToList();
                                            

        public CustomButton(string name, string imageName, float cooldown, bool isTargetButton, string[] allowedRoles, 
                                                        string[] rolesCantTarget, Action<CustomActionButton, bool> onClicked)
        {
            this.name = name;
            this.imageName = imageName;
            this.cooldown = cooldown;
            this.targetButton = isTargetButton;
            this.buttonForGhosts = false;
            this.buttonTargetsGhosts = false;
            this.caresAboutMoving = true;
            this.allowedRoles = allowedRoles.ToList();
            this.canTargetSelf = false;
            this.rolesCantTarget = rolesCantTarget.ToList();

            this.onClicked = onClicked;
        }

        public CustomButton() {
            allowedRoles = new List<string>();
            rolesCantTarget = new List<string>();
        }

        public bool RoleAllowed(string role) {
            return allowedRoles.Contains(role);
        }

        public void OnClicked(CustomActionButton button, bool success) { }
    }

    [Il2CppItem]
    class CustomActionButton : ActionButton {
        public float maxCooldown;
        private DateTime lastUse;
        public CustomButton buttonData;

        public PlayerControl curTarget;

        public void Setup(CustomButton buttonData) {
            this.buttonData = buttonData;
            this.maxCooldown = this.buttonData.cooldown;
        }

        public void Start()
        {
            base.Start();

            this.isCoolingDown = true;
            this.lastUse = DateTime.UtcNow;

            Texture2D tex2d = new Texture2D(110, 110);
            Stream myStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(buttonData.imageName);
            myStream.Position = 0;
            byte[] buttonTexture = new byte[myStream.Length];
            for (int i = 0; i < myStream.Length;)
                i += myStream.Read(buttonTexture, i, Convert.ToInt32(myStream.Length) - i);
            ImageConversion.LoadImage(tex2d, buttonTexture, false);
            this.graphic.sprite = Sprite.Create(tex2d, new Rect(0, 0, 110, 110), new Vector2(55, 55));
        }

        public void Update() {
            TimeSpan timeLeft = DateTime.UtcNow - lastUse;
            int timeRemaining = (int)Math.Ceiling((double)new decimal(maxCooldown - timeLeft.TotalMilliseconds / 1000f));
            SetCoolDown(timeRemaining < 0 ? 0 : timeRemaining, maxCooldown);

            SetTarget(DillyzUtil.getClosestPlayer(PlayerControl.LocalPlayer, this.buttonData.rolesCantTarget, 
                    GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], !this.buttonData.buttonTargetsGhosts, 
                    this.buttonData.canTargetSelf));
        }

        public bool CanUse() {
            return MeetingHud.Instance == null && buttonData.RoleAllowed(DillyzUtil.getRoleName(PlayerControl.LocalPlayer));
        }

        public void DoClick() {
            if (!base.isActiveAndEnabled || (this.curTarget == null && this.buttonData.targetButton) || this.isCoolingDown &&
                PlayerControl.LocalPlayer.Data.IsDead != this.buttonData.buttonForGhosts || (this.buttonData.caresAboutMoving && !PlayerControl.LocalPlayer.CanMove))
            { 
                this.buttonData.OnClicked(this, false);
                return;
            }

            this.lastUse = DateTime.UtcNow;
            this.buttonData.OnClicked(this, true);
            SetTarget(null);
        }

        public void SetTarget(PlayerControl player) {
            if (this.curTarget != null && this.curTarget != player)
                this.curTarget.ToggleHighlight(false, PlayerControl.LocalPlayer.Data.Role.TeamType);
            this.curTarget = player;
            if (curTarget != null)
            {
                this.curTarget.ToggleHighlight(true, PlayerControl.LocalPlayer.Data.Role.TeamType);
                base.SetEnabled();
                return;
            }
            base.SetDisabled();
        }
    }
}
