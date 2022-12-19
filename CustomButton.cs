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
        public string imageName = "DillyzRoleApi_Rewritten.Assets.kill.png";
        public float cooldown = 30;
        public bool targetButton = false;
        public bool buttonForGhosts = false;
        public bool buttonTargetsGhosts = false;
        public bool caresAboutMoving = true;
        public List<string> allowedRoles;
        public bool canTargetSelf = false;
        public List<string> rolesCantTarget;

        public byte globalId;
        public byte topGlobalId = 0;

        public Action<CustomActionButton, bool> onClicked;

        public static Dictionary<string, CustomButton> buttonMap = new Dictionary<string, CustomButton>();
        public static CustomButton getButtonByName(string name) => buttonMap.ContainsKey(name) ? buttonMap[name] : null;
        public static void addButton(string name, string imageName, float cooldown, bool isTargetButton, string[] allowedRoles, string[] rolesCantTarget,
                        Action<CustomActionButton, bool> onClicked) => buttonMap[name] = new CustomButton(name, imageName, cooldown, isTargetButton, allowedRoles,
                                                                                                                                        rolesCantTarget, onClicked);
        public static List<CustomButton> AllCustomButtons => buttonMap.Values.ToArray().ToList();

        public static CustomButton getById(byte targetId) {
            foreach (CustomButton button in AllCustomButtons)
                if (button.globalId == targetId)
                    return button;
            return null;
        }

        public CustomButton(string name, string imageName, float cooldown, bool isTargetButton, string[] allowedRoles,
                                                        string[] rolesCantTarget, Action<CustomActionButton, bool> onClicked)
        {
            this.globalId = topGlobalId++;
            HarmonyMain.Instance.Log.LogInfo($"Button ID {this.globalId} exists under {name}.");

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

        private bool ready = false;

        public void Setup(byte buttonId)
        {
            HarmonyMain.Instance.Log.LogInfo("buttonnnnn1");
            this.buttonData = CustomButton.getById(buttonId);
            HarmonyMain.Instance.Log.LogInfo("buttonnnnn12");
            this.maxCooldown = this.buttonData.cooldown;

            HarmonyMain.Instance.Log.LogInfo("buttonnnnn123");
            // start code
            this.isCoolingDown = true;
            this.lastUse = DateTime.UtcNow;

            HarmonyMain.Instance.Log.LogInfo("buttonnnnn1234");
            Texture2D tex2d = new Texture2D(110, 110);
            HarmonyMain.Instance.Log.LogInfo("buttonnnnn12345 " + buttonData.imageName);
            Stream myStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(buttonData.imageName);
            if (myStream != null)
            {
                HarmonyMain.Instance.Log.LogInfo("buttonnnnn123456 " + myStream.Length);

                myStream.Position = 0;
                byte[] buttonTexture = new byte[myStream.Length];
                HarmonyMain.Instance.Log.LogInfo("buttonnnnn1234567");
                for (int i = 0; i < myStream.Length;)
                    i += myStream.Read(buttonTexture, i, Convert.ToInt32(myStream.Length) - i);
                HarmonyMain.Instance.Log.LogInfo("buttonnnnn12345678");
                ImageConversion.LoadImage(tex2d, buttonTexture, false);
                HarmonyMain.Instance.Log.LogInfo("buttonnnnn123456789");
                this.graphic.sprite = Sprite.Create(tex2d, new Rect(0, 0, 110, 110), new Vector2(0, 0));
            }
            else
                HarmonyMain.Instance.Log.LogInfo("buttonnnnn123456 null");

            HarmonyMain.Instance.Log.LogInfo("buttonnnnn123456789A");
            base.Start();

            HarmonyMain.Instance.Log.LogInfo("buttonnnnn123456789B");
            ready = true;
        }

        public void Start()
        {
            // block it
        }

        public void Update() {
            if (!ready)
                return;

            TimeSpan timeLeft = DateTime.UtcNow - lastUse;
            int timeRemaining = (int)Math.Ceiling((double)new decimal(maxCooldown - timeLeft.TotalMilliseconds / 1000f));
            SetCoolDown(timeRemaining < 0 ? 0 : timeRemaining, maxCooldown);

            SetTarget(DillyzUtil.getClosestPlayer(PlayerControl.LocalPlayer, this.buttonData.rolesCantTarget, 
                    GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], !this.buttonData.buttonTargetsGhosts, 
                    this.buttonData.canTargetSelf));
        }

        public bool CanUse()
        {
            if (!ready)
                return false;

            return MeetingHud.Instance == null && buttonData.RoleAllowed(DillyzUtil.getRoleName(PlayerControl.LocalPlayer));
        }

        public void DoClick()
        {
            if (!ready)
                return;

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

        public void SetTarget(PlayerControl player)
        {
            if (!ready)
                return;

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
