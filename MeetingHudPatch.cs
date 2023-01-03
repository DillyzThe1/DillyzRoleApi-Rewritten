using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DillyzRoleApi_Rewritten
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    class MeetingHudPatch
    {

        public static void Postfix() {
            CustomButton.ResetAllButtons();
        }
    }
}
