# DillyzRoleApi-Rewritten
 A full rewrite for DillyzRoleApi, made for v2022.12.14 & up!
 
# Installation
## The API itself.
Clone your Among Us install folder.<br>
In the clone of the install folder, do either method A or B:<br>
<br>
### Method A (easiest)
Head over to the <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/releases/latest/">latest release</a> and download `DillyzRoleApi_2.x.x_BepInExBundle.zip`.<br>
Extract the files and put it in your Among Us install's clone.<br>
Boot up the game using the exe in the Among Us clone.
### Method B
Get a <a href="https://builds.bepinex.dev/projects/bepinex_be/577/BepInEx_UnityIL2CPP_x86_ec79ad0_6.0.0-be.577.zip">compatible version of BepInEx</a>.
Extract these files and put it in your Among Us install's clone.<br>
Make a file called `steam_appid.txt` and make it contain `945360`.<br>
Boot up the game using the exe in the Among Us clone.<br>
After Among Us opens, close it immediantly.<br>
Head over to the <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/releases/latest/">latest release</a> and download `DillyzRoleApi_2.x.x.dll`.<br>
Make sure to also download the <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/releases/latest/">latest release</a> of `ClassicRolePackage_1.x.x.dll`<br>
Put the DLL in `BepInEx/plugins`.<br>
Boot up the game using the exe in the Among Us clone.

## Roles & Mods
Currently, the 

# Modding with the API
*WARNING: THE DOCUMENTATION IS UNFINISHED AND ONLY HAS A TEMPORARY TUTORIAL TO MAKE A ROLE WITH CUSTOM BUTTONS. BASIC DOCUMENTATION SHOULD BE FINISHED BY JANUARY 1st, 2023.*<br>
Making a clone of ClassicRolePackage

# Disclaimer
This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC.<br>
Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.<br>
To make usage of this API, you must agree to and follow the <a href="https://www.innersloth.com/among-us-mod-policy/">Innersloth mod policy</a>.<br>
I am not reponsible for any damage, harm, etc caused by using this API in any form.

*Note: You do not need to manually add the "mod stamp" within your usage of the API, as ShowModStamp() is <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/blob/main/HarmonyMain.cs#L64">already called prior</a>.*