using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using BepInEx.IL2CPP.Utils;
using Hazel;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    public class DillyzUtil
    {
        public static CustomButton addButton(Assembly epicAssemblyFail, string name, string imageName, float cooldown, bool isTargetButton, string[] allowedRoles,
                                                                                    string[] rolesCantTarget, Action<KillButtonCustomData, bool> onClicked) =>
                                    CustomButton.addButton(epicAssemblyFail, name, imageName, cooldown, isTargetButton, allowedRoles, rolesCantTarget, onClicked);
        public static CustomRole createRole(String name, String subtext, bool nameColor, bool nameColorPublic, Color32 roleColor, bool canSeeTeam, CustomRoleSide side,
                    VentPrivilege ventPrivilege, bool canKill, bool showEjectText) =>
                                 CustomRole.createRole(name, subtext, nameColor, nameColorPublic, roleColor, canSeeTeam, side, ventPrivilege, canKill, showEjectText);

        public static String getRoleName(PlayerControl player)
        {
            if (player == null || player.Data == null)
                return "Crewmate";

            if (CustomRole.getRoleName(player.PlayerId) != "" && CustomRole.getRoleName(player.PlayerId) != null)
                return CustomRole.getRoleName(player.PlayerId);

            switch (player.Data.RoleType)
            {
                case RoleTypes.Crewmate:
                    return "Crewmate";
                case RoleTypes.CrewmateGhost:
                    return "Crewmate";
                case RoleTypes.Impostor:
                    return "Impostor";
                case RoleTypes.ImpostorGhost:
                    return "Impostor";
                case RoleTypes.Scientist:
                    return "Scientist";
                case RoleTypes.Engineer:
                    return "Engineer";
                case RoleTypes.GuardianAngel:
                    return "GuardianAngel";
                case RoleTypes.Shapeshifter:
                    return "ShapeShifter";
            }

            return "Crewmate";
        }

        public static CustomRoleSide roleSide(PlayerControl player)
        {
            if (player == null || player.Data == null)
            {
                DillyzRoleApiMain.Instance.Log.LogInfo("hey noob ur mod broke");
                return CustomRoleSide.Crewmate;
            }
            if (CustomRole.getRoleName(player.PlayerId) != "" && CustomRole.getRoleName(player.PlayerId) != null)
                return CustomRole.getByName(CustomRole.getRoleName(player.PlayerId)).side;
            if (player.Data.RoleType == RoleTypes.Impostor || player.Data.RoleType == RoleTypes.ImpostorGhost || player.Data.RoleType == RoleTypes.Shapeshifter)
                return CustomRoleSide.Impostor;
            return CustomRoleSide.Crewmate;
        }

        public static Color32 roleColor(PlayerControl player, bool nameText)
        {
            if (player == null || player.Data == null)
                return nameText ? CustomPalette.White : CustomPalette.CrewmateBlue;
            if (CustomRole.getRoleName(player.PlayerId) != "" && CustomRole.getRoleName(player.PlayerId) != null)
                return CustomRole.getByName(CustomRole.getRoleName(player.PlayerId)).roleColor;

            switch (player.Data.RoleType) {
                case RoleTypes.Impostor:
                    return CustomPalette.ImpostorRed;
                case  RoleTypes.ImpostorGhost:
                    return CustomPalette.ImpostorRed;
                case RoleTypes.Shapeshifter:
                    return CustomPalette.ShapeShifterCrimson;
                case RoleTypes.Engineer:
                    return CustomPalette.EngineerOrange;
                case RoleTypes.Scientist:
                    return CustomPalette.ScientistTeal;
                case RoleTypes.GuardianAngel:
                    return CustomPalette.GuardianAngleLightBlue;
            }

            return player.Data.Role.IsImpostor ? CustomPalette.ImpostorRed : (nameText ? CustomPalette.White : CustomPalette.CrewmateBlue);
        }

        public static string roleText(PlayerControl player)
        {
            if (player == null)
                return "\n\nInvalid Player! (1)";
            if (player.Data == null)
                return "\n\nInvalid Player! (2)";
            string rc = colorToHex(roleColor(player, false));

            if (CustomRole.getRoleName(player.PlayerId) != "" && CustomRole.getRoleName(player.PlayerId) != null)
            {
                CustomRole cr = CustomRole.getByName(CustomRole.getRoleName(player.PlayerId));
                if (cr.name != "" && cr.name != null)
                    return $"\n\n<{rc}>You're {cr.a_or_an} {cr.name}. {cr.subtext}</color>";
            }

            switch (player.Data.RoleType)
            {
                case RoleTypes.Impostor:
                    return $"\n\n<{rc}>You're an Impostor.</color>";
                case RoleTypes.ImpostorGhost:
                    return "";
                case RoleTypes.Shapeshifter:
                    return $"\n\n<{rc}>You're a Shapeshifter. Shapeshift to decieve the crew.</color>";
                case RoleTypes.Engineer:
                    return $"\n\n<{rc}>You're an Engineer. Use the vents to help the crew.</color>";
                case RoleTypes.Scientist:
                    return $"\n\n<{rc}>You're a Scientist. Use vitals to track the crew.</color>";
                case RoleTypes.GuardianAngel:
                    return $"\n\n<{rc}>You're a Guardian Angel. Protect the crew from kills.</color>";
                case RoleTypes.CrewmateGhost:
                    return "";
            }

            return "\n\nYou're a Crewmate.";
        }

        public static bool templateRole(PlayerControl player) {
            String name = getRoleName(player);
            return name == "Impostor" || name == "Crewmate";
        }

        public static PlayerControl findPlayerControl(byte playerId) {
            if (playerId == 255)
                return null;

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == playerId)
                    return player;
            return null;
        }

        public static Color32 copyColor(Color32 ogColor) => new Color32(ogColor.r, ogColor.g, ogColor.b, ogColor.a);
        private static String convDigitToHexNum(byte num) {
            if (num < 10)
                return num.ToString();
            switch (num % 16) {
                case 10: return "A";
                case 11: return "B";
                case 12: return "C";
                case 13: return "D";
                case 14: return "E";
                case 15: return "F";
            }
            return "0";
        }
        private static String convNumToHexNum(byte num) => convDigitToHexNum((byte)(num / 16)) + convDigitToHexNum((byte)(num % 16));
        public static String colorToHex(Color32 ogColor) => "#" + convNumToHexNum(ogColor.r) + convNumToHexNum(ogColor.g) + convNumToHexNum(ogColor.b);


        public static PlayerControl getClosestPlayer(PlayerControl centerPlayer)
        {
            return getClosestPlayer(centerPlayer, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance]);
        }
        public static PlayerControl getClosestPlayer(PlayerControl centerPlayer, double mindist)
        {
            return getClosestPlayer(centerPlayer, null, mindist, true, false);
        }

        public static PlayerControl getClosestPlayer(PlayerControl centerPlayer, System.Collections.Generic.List<String> roleFilters, double mindist, bool shouldBeAlive, bool canTargetSelf) {
            System.Collections.Generic.List<PlayerControl> welcomeOldPlayers = PlayerControl.AllPlayerControls.ToArray().ToList();
            PlayerControl close = canTargetSelf ? PlayerControl.LocalPlayer : null;
            double playerDist = mindist;

            if (roleFilters != null)
                welcomeOldPlayers.RemoveAll(x => roleFilters.Contains(getRoleName(x)));
            welcomeOldPlayers.RemoveAll(x => x.Data.IsDead == shouldBeAlive);
            welcomeOldPlayers.RemoveAll(x => x.inVent);

            foreach (PlayerControl player in welcomeOldPlayers)
            {
                if (player == centerPlayer)
                    continue;
                double dist = getDist(centerPlayer.GetTruePosition(), player.GetTruePosition());
                if (dist >= playerDist)
                    continue;
                playerDist = dist;
                close = player;
            }
            return close;
        }

        // A workaround for killing.
        public static void RpcCommitAssassination(PlayerControl assassinator, PlayerControl target)
        {
            commitAssassination(assassinator, target);

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.Assassinate, Hazel.SendOption.None, -1);
            writer.Write(assassinator.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
        public static void commitAssassination(PlayerControl assassinator, PlayerControl target)
        {
            //DillyzRoleApiMain.Instance.Log.LogInfo(assassinator.KillAnimations.Length + " kill anims");
            ///foreach (KillAnimation killanim in assassinator.KillAnimations)
            //    DillyzRoleApiMain.Instance.Log.LogInfo("Kill animation " + killanim.name + " found!");
            DillyzRoleApiMain.Instance.Log.LogInfo(assassinator.name + " " + target.name);

            bool isKiller = assassinator.PlayerId == PlayerControl.LocalPlayer.PlayerId;
            bool isVictim = target.PlayerId == PlayerControl.LocalPlayer.PlayerId;

            if (target.protectedByGuardian)
            {
                target.protectedByGuardianThisRound = true;
                string rolename = DillyzUtil.getRoleName(PlayerControl.LocalPlayer);
                if (isKiller || rolename == "GuardianAngel")
                {
                    target.ShowFailedMurder();
                    assassinator.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown) / 2f);
                }
                else
                    target.RemoveProtection();

                if (rolename == "GuardianAngel")
                    DestroyableSingleton<AchievementManager>.Instance.OnProtectACrewmate();
                return;
            }
            if (isKiller)
            {
                if (Constants.ShouldPlaySfx())
                    SoundManager.Instance.PlaySound(assassinator.KillSfx, false, 0.8f, null);

                assassinator.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
            }


            //CustomRoleSide roleSideUrOn = DillyzUtil.roleSide(target);
            KillAnimation kil = assassinator.KillAnimations[UnityEngine.Random.Range(0, assassinator.KillAnimations.Length - 1)];

            if (isVictim) {
                if (Minigame.Instance != null) {
                    try {
                        Minigame.Instance.Close();
                        Minigame.Instance.Close();
                    } catch { }
                }
                DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(assassinator.Data, target.Data);
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                target.cosmetics.SetNameMask(false);
                target.RpcSetScanner(false);
            }

            assassinator.MyPhysics.StartCoroutine(DoCustomKill(kil, assassinator, target));
        }

        private static IEnumerator DoCustomKill(KillAnimation killanim, PlayerControl assassinator, PlayerControl target) {
            FollowerCamera curCamera = Camera.main.GetComponent<FollowerCamera>();
            bool involved = PlayerControl.LocalPlayer.PlayerId == assassinator.PlayerId || PlayerControl.LocalPlayer.PlayerId == target.PlayerId;

            // no move
            KillAnimation.SetMovement(assassinator, false);
            KillAnimation.SetMovement(target, false);

            // ded body
            DeadBody deadBody = GameObject.Instantiate(killanim.bodyPrefab);
            deadBody.enabled = false;
            deadBody.ParentId = target.PlayerId;
            target.SetPlayerMaterialColors(deadBody.bodyRenderer);
            target.SetPlayerMaterialColors(deadBody.bloodSplatter);
            Vector3 vector = target.transform.position + killanim.BodyOffset;
            vector.z = vector.y / 1000f;
            deadBody.transform.position = vector;

            if (involved) {
                curCamera.Locked = true;
                ConsoleJoystick.SetMode_Task();
                if (PlayerControl.LocalPlayer.AmOwner)
                    PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
            }

            target.Die(DeathReason.Kill, true);

            yield return assassinator.MyPhysics.Animations.CoPlayCustomAnimation(killanim.BlurAnim);

            assassinator.NetTransform.SnapTo(target.transform.position);
            assassinator.MyPhysics.Animations.PlayIdleAnimation();

            // move
            KillAnimation.SetMovement(assassinator, true);
            KillAnimation.SetMovement(target, true);

            deadBody.enabled = true;
            if (involved)
                curCamera.Locked = false;

            //CustomRole.setRoleName(target.PlayerId, "");

            yield break;
        }

        public static void RpcSetRole(PlayerControl player, string role) {
            DillyzRoleApiMain.Instance.Log.LogInfo($"setting {player.name} as {role}");
            CustomRole.setRoleName(player.PlayerId, role);

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.SetRole, Hazel.SendOption.None, -1);
            writer.Write(player.PlayerId);
            writer.Write(role);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static double getDist(Vector2 p1, Vector2 p2)
        {
            // maths
            return Math.Sqrt((p1[0] - p2[0]) * (p1[0] - p2[0]) + (p1[1] - p2[1]) * (p1[1] - p2[1]));
        }

        public static bool InFreeplay()
        {
            return AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
        }
        public static bool InGame()
        {
            return AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started;
        }

        // Make sure your sprite is an Embedded Resource, compile, then check the console. You'll get a sprite path.
        public static Sprite getSprite(Assembly assembly, string spritePath) {
            Stream myStream = assembly.GetManifestResourceStream(spritePath);
            if (myStream != null)
            {
                myStream.Position = 0;
                byte[] buttonTexture = new byte[myStream.Length];
                for (int i = 0; i < myStream.Length;)
                    i += myStream.Read(buttonTexture, i, Convert.ToInt32(myStream.Length) - i);

                Texture2D tex2d = new Texture2D(110, 110);
                ImageConversion.LoadImage(tex2d, buttonTexture, false);
                return Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.one * 0.5f, 100f);
            }
            else
                DillyzRoleApiMain.Instance.Log.LogError("Sprite image \"" + spritePath + "\" could not be found!");

            return null;
        }

        [Obsolete("ok")]
        public static AudioClip getSound(Assembly assembly, string soundPath, string soundId, int freq)
        {
            return getSound(assembly, soundPath, soundId, freq);
        }
        public static AudioClip getSound(Assembly assembly, string soundPath, string soundId) {
            if (!soundPath.EndsWith(".wav"))
            {
                DillyzRoleApiMain.Instance.Log.LogError("Only .wav files are supported for sound!");
                return null;
            }
            DillyzRoleApiMain.Instance.Log.LogInfo("wav");

            Stream myStream = assembly.GetManifestResourceStream(soundPath);
            if (myStream != null)
            {
                DillyzRoleApiMain.Instance.Log.LogInfo("wav1");
                myStream.Position = 0;
                byte[] audioBytes = new byte[myStream.Length];
                for (int i = 0; i < myStream.Length;)
                    i += myStream.Read(audioBytes, i, Convert.ToInt32(myStream.Length) - i);

                DillyzRoleApiMain.Instance.Log.LogInfo("wav2");
                // https://sites.google.com/site/musicgapi/technical-documents/wav-file-format
                UInt16 format = BitConverter.ToUInt16(audioBytes, 20);
                if (format == 1 || format == 65534)
                {
                    DillyzRoleApiMain.Instance.Log.LogError("Sound file at \"" + soundPath + "\" is of wrong format! (foramt " + format + ")");
                    return null;
                }
                DillyzRoleApiMain.Instance.Log.LogInfo("wav3");
                UInt16 depth = BitConverter.ToUInt16(audioBytes, 52);

                int c1 = BitConverter.ToInt32(audioBytes, 16);
                int h = 16 + 4 + c1 + 4;
                int samples = (int)BitConverter.ToInt32(audioBytes, 60);
                DillyzRoleApiMain.Instance.Log.LogInfo(samples);

                for (int a = 0; a < 100; a++)
                {
                    DillyzRoleApiMain.Instance.Log.LogInfo(a + " -> " + ((int)BitConverter.ToInt32(audioBytes, a)) + " || " + ((int)BitConverter.ToInt16(audioBytes, a)) +
                    " || " + ((int)BitConverter.ToUInt32(audioBytes, a)) + " || " + ((int)BitConverter.ToUInt16(audioBytes, a)));
                }

                DillyzRoleApiMain.Instance.Log.LogInfo("wav4" + depth);
                float[] soundFloats;

                int s = 0, h2 = 0, f = 0, cs = 0, p = 0;
                switch (depth) {
                    case 16:
                        DillyzRoleApiMain.Instance.Log.LogInfo("wav5");
                        s = BitConverter.ToInt32(audioBytes, h);
                        h2 = h + sizeof(int); 
                        f = sizeof(Int16); 
                        cs = s / f;
                        p = 0;
                        soundFloats = new float[cs];
                        while (p < cs)
                        {
                            soundFloats[p] = (float)BitConverter.ToInt16(audioBytes, p * f + h2) / Int16.MaxValue;
                            p++;
                        }
                        break;
                    case 32:
                        DillyzRoleApiMain.Instance.Log.LogInfo("wav5");
                        s = BitConverter.ToInt32(audioBytes, h);
                        h2 = h + sizeof(int); 
                        f = sizeof(float); 
                        cs = s / f; 
                        p = 0;
                        soundFloats = new float[cs];
                        while (p < cs) {
                            soundFloats[p] = (float)BitConverter.ToInt32(audioBytes, p * f + h2) / Int32.MaxValue;
                            p++;
                        }
                        break;
                    default:
                        DillyzRoleApiMain.Instance.Log.LogError("Sound file \"" + soundPath + "\" is of unsupported depth! (" + depth + "bit)");
                        return null;

                }
                DillyzRoleApiMain.Instance.Log.LogInfo("wav6");
                AudioClip clip = AudioClip.Create(soundId, soundFloats.Length, (int)BitConverter.ToInt16(audioBytes, 58), samples, false);
                clip.SetData(soundFloats, 0);
                return clip;
            }
            else
                DillyzRoleApiMain.Instance.Log.LogError("Sound file at \"" + soundPath + "\" could not be found!");

            return null;
        }

        public static void AddRpcCall(string rpcName, Action<MessageReader> callback) {
            DillyzRoleApiMain.Instance.Log.LogInfo("Added RPC callback for " + rpcName + ".");
            CustomRpcHandler.customRpcCallbacks.Add(new CustomRpcCallback(rpcName, callback));
        }

        public static void InvokeRPCCall(string rpcName, Action<MessageWriter> writingCallback) {
            if (writingCallback == null)
            {
                DillyzRoleApiMain.Instance.Log.LogError(rpcName + " had a null writingCallback! (Did you forget delegate(MessageWriter writer) {}, you idiot?)" +
                    "\n     (Also, in the callback you need to write data with writer.Write(yourvariable);)");
                return;
            }

            foreach (CustomRpcCallback callback in CustomRpcHandler.customRpcCallbacks)
                if (callback.rpcName == rpcName) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRpc.CustomRPCCall, SendOption.None, -1);
                    writer.Write(rpcName);
                    if (writingCallback != null)
                        writingCallback(writer);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    return; 
                }

            DillyzRoleApiMain.Instance.Log.LogError("Alright, you're trolling. You're. TROLLING. The RPC you're calling is null. " + rpcName + " doesn't EXIST! Are you ok?!");
        }

        public static Color color32ToColor(Color32 color32)
        {
            return new Color(color32.r / 255f, color32.g / 255f, color32.b / 255f, 1f);
        }

        // TODO: Document this function later!
        public static string SafeSubString(string str, int start, int length)
        {
            if (length >= str.Length)
                return str;
            if (length < 0)
                return "";
            if (length == 0)
                return "";

            return str.Substring(start, length);

        }
        public static List<PlayerControl> GetAllOfRole(string rolename)
        {
            List<PlayerControl> pcs = PlayerControl.AllPlayerControls.ToArray().ToList();
            pcs.RemoveAll(x => getRoleName(x) != rolename);
            return pcs;
        }
    }
}
