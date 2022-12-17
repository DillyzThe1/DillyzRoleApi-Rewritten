﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DillyzRoleApi_Rewritten {
    class CustomPlayerData {
        public static List<CustomPlayerData> allPlayerData => new List<CustomPlayerData>();
        public static void create(PlayerControl baseControl) => allPlayerData.Add(new CustomPlayerData(baseControl));
        public static void destroy(PlayerControl playerControl) => allPlayerData.Remove(findByPlayerControl(playerControl));

        public static CustomPlayerData findByPlayerControl(PlayerControl playerControl) {
            foreach (CustomPlayerData customdata in allPlayerData)
                if (customdata.id == playerControl.PlayerId)
                    return customdata;
            return null;
        }

        public byte id;         // Let's identify the player.
        public String roleName;

        public PlayerControl playerControl {
            get {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    if (player.PlayerId == id)
                        return player;
                return null;
            }
        }

        public CustomPlayerData(PlayerControl baseControl) {
            this.id = baseControl.PlayerId;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
    class PlayerControl_StartPatch {
        public static void Postfix(PlayerControl __instance) {
            CustomPlayerData.create(__instance);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnDestroy))]
    class PlayerControl_DestroyPatch {
        public static void Prefix(PlayerControl __instance) {
            CustomPlayerData.destroy(__instance);
        }
    }


}
