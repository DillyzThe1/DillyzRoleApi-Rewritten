# DillyzRoleApi-Rewritten
 A full rewrite for DillyzRoleApi, made for v2022.12.14 & up!<br>
*NOTE: DillyzRoleApi v2.2.0 for Among Us v2023.2.28 will be in the works by April.*
 
# Installation
## Mod Manager
Head over to the [mod manager's releases page](https://github.com/DillyzThe1/DRAPI-Mod-Manager/releases) & download the installer exe.<br>
If windows warns you about the program, remember that this is fully open source so you can read what you don't trust. Just hit run anyway.<br>
Follow through the installation and open up the program (likely through your desktop icon).<br>
Install the client with the "reinstall" button and browse mods.<br>
Hit "launch" and wait for the first boot-up procedure to happen.<br>
Enjoy!<br>
<br>
*Note: you must always launch through the launcher to get the modded variant of Among Us running.*
## Manually
Clone your Among Us install folder.<br>
In the clone of the install folder, do either method A or B:<br>
<br>
Head over to the <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/releases/latest/">latest release</a> and download [`DillyzRoleApi_2.x.x_BepInExBundle.zip`](../../releases/download/2.0.0/DillyzRoleApi_2.0.0_BepInExBundle.zip).<br>
Extract the files and put it in your Among Us install's clone.<br>
Boot up the game using the exe in the Among Us clone.

## Roles & Mods
You may download the API alongside and officially recognized mods that have been listed over at [DRAPI-Mod-Manager](https://github.com/DillyzThe1/DRAPI-Mod-Manager/releases/latest/).<br>
*NOTE: "Official" is relevant to me as the API and mod manager creator, as I state that I am not associated with Innersloth.*
Please use the mod manager rather than manual downloads.<br>
If you wish to have a mod you like added to the launcher, [submit an issue](https://github.com/DillyzThe1/DRAPI-Mod-Manager/issues/new) in the [appropiate repository](https://github.com/DillyzThe1/DRAPI-Mod-Manager) detailing the mod name and functionality so I can reach out to the developer.<br>
<br>
All official role packs and mods will be included in the BepInEx bundle.<br>
Source code to each mod:
- [ClassicRolePackage](https://github.com/DillyzThe1/ClassicRolePackage)
- [DillyzLegacyPack](https://github.com/DillyzThe1/DillyzLegacyPack)

*Note: "Official" means developed by DillyzThe1, who owns the API and the associated launcher. See disclaimer below.*

## All Versions
| Among Us<br>Version | DillyzRoleApi<br>Version | BepInEx + DillyzRoleApi<br>+ All Official Packs |  BepInEx<br>Build  | Standalone DLL |
|------------------|-----------------------|--------------------------------------------------|-----------------|----------------|
|**REGULAR BUILDS**| | | | |
| 2022.12.14 | Latest Commit | Unavailable | [[Download]](https://builds.bepinex.dev/projects/bepinex_be/577/BepInEx_UnityIL2CPP_x86_ec79ad0_6.0.0-be.577.zip) | [[Download]](../../raw/main/notes%20n%20stuff/DillyzRoleApi-Rewritten-Latest-Commit.dll) |
| 2022.12.14 | v2.1.0 | [[Download]](../../releases/download/2.1.0/DillyzRoleApi_2.1.0_BepInExBundle.zip) | [[Download]](https://builds.bepinex.dev/projects/bepinex_be/577/BepInEx_UnityIL2CPP_x86_ec79ad0_6.0.0-be.577.zip) | [[Download]](../../releases/download/2.1.0/DillyzRoleApi-Rewritten.dll) |
| 2022.12.14 | v2.0.0 | [[Download]](../../releases/download/2.0.0/DillyzRoleApi_2.0.0_BepInExBundle.zip) | [[Download]](https://builds.bepinex.dev/projects/bepinex_be/577/BepInEx_UnityIL2CPP_x86_ec79ad0_6.0.0-be.577.zip) | Unavailable |
| <center>**LEGACY BUILDS**</center>    |  | <b>BepInEx-Reactor + DillyzRoleApi<br>+ Reactor API</b> |   |         |
| 2021.4.12 & 2021.4.14 | v1.3.0 | [[Download]](https://github.com/DillyzThe1/DillyzRoleApi/releases/download/1.3.0/DillyzRoleAPI+BepInEx+Reactor.1.3.0.zip) | Unknown | [[Download]](https://github.com/DillyzThe1/DillyzRoleApi/releases/download/1.3.0/DillyzRolesAPI-2021.4.12s.dll) |
| 2021.4.12 & 2021.4.14 | v1.2.0 | [[Download]](https://github.com/DillyzThe1/DillyzRoleApi/releases/download/1.2.0/DillyzRoleAPI+BepInEx+Reactor.1.2.0.zip) | Unknown | [[Download]](https://github.com/DillyzThe1/DillyzRoleApi/releases/download/1.2.0/DillyzRolesAPI-2021.4.12s.dll) |
| 2021.4.12 & 2021.4.14 | v1.1.0 | [[Download]](https://github.com/DillyzThe1/DillyzRoleApi/releases/download/1.1.0/DillyzRoleAPI+BepInEx+Reactor.zip) | Unknown | [[Download]](https://github.com/DillyzThe1/DillyzRoleApi/releases/download/1.1.0/DillyzRolesAPI-2021.4.12s.dll) |
| 2021.4.2 | v1.0.0 | Unavailable | Unknown | Unavailable |

# Modding with the API
*WARNING: THE DOCUMENTATION IS UNFINISHED AND ONLY CONTAINS UNFINISHED ROLE EXAMPLES AND UTILITY DOCUMENTATION.*<br>
*PLEASE USE THE DISCORD LINK ON THE WIKI PAGE FOR MORE HELP!*<br>
<br>
[[DillyzRoleApi Wiki]](https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/wiki)

# Disclaimer
This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC.<br>
Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.<br>
To make usage of this API, you must agree to and follow the <a href="https://www.innersloth.com/among-us-mod-policy/">Innersloth mod policy</a>.<br>
I am not reponsible for any damage, harm, etc caused by using this API in any form.

*Note: You do not need to manually add the "mod stamp" within your usage of the API, as ShowModStamp() is <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/blob/main/DillyzRoleApiMain.cs#L60">already called prior</a>.*
