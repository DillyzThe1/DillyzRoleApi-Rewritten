using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DillyzRoleApi_Rewritten
{
    public class CustomPalette
    {
        // ROLE NAME COLORS
        public static Color32 White => new Color32(253, 253, 253, 255);
        public static Color32 ImpostorRed => new Color32(253, 25, 25, 255);
        public static Color32 CrewmateBlue => new Color32(139, 253, 253, 255);
        public static Color32 LoneWolfGray => new Color32(125, 125, 165, 255);

        // BASE GAME ROLE MODIFIERS
        public static Color32 EngineerOrange => new Color32(255, 145, 35, 255);
        public static Color32 ScientistTeal => new Color32(105, 150, 255, 255);
        public static Color32 GuardianAngleLightBlue => new Color32(165, 225, 255, 255);
        public static Color32 ShapeShifterCrimson => new Color32(190, 30, 55, 255);

        // BUTTONS
        public static Color32 KillButtonTextOutline = new Color32(219, 37, 0, 255);
        public static Color32 PassiveButtonTextOutline = new Color32(0, 0, 0, 255);

        // UI
        //public static Color32 GameSettingDeselectedColor = new Color32(255, 255, 255, 255);
        public static Color32 GameSettingSelectedColor = new Color32(0, 255, 0, 255);
        [Obsolete("This color may not be accurate to the actual game.", false)]
        public static Color32 CheckboxSelectedColor = new Color32(255, 225, 0, 255);
    }
}
