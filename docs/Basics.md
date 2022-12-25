# API Basics
NOTE!!!!!! THIS WAS WRITTEN BEFORE I COULD TEST GETTING DOTNET FROM GITHUB.COM, SO IK IT'S A BIT WONKY RN!!!!!!!<br>
THE WIKI VERSION WILL ACTAULLY BE MORE POLISHED AND WHATEVER THAN THIS!!!!!!<br>
<br>
ANY PROBLEMS YOU HAVE SHOULD BE COMPLAINED ABOUT ON THE <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/issues/">ISSUES PAGE</a> OR TALKED ABOUT IN MY <a href="https://discord.gg/49NFTwcYgZ">DISCORD SERVER</a>.<br>
THANK YOU FOR CHOOSING MY API, I'LL BE HAPPY TO HELP YOU USE IT!<br>
*just please don't bother me during december 25th unless i have myself set to online*

## Introduction
So, you've stumbled upon my API and wish to use it for your mods.<br>
In this tutorial, I will be showing you how to setup a full recreation of <a href="/">the classic modes 1 rolepackage</a> using my simple API.<br>
*Yes, this is all tested. I not only actively developed the API and used the functions, but I also made the rolepackage at the same time as this documentation.*<br>
<br>
Requirements:
- Windows 10 or 11 (Maybe Mac or Linux, but I cannot say until someone confirms.)
- <a href="https://visualstudio.microsoft.com/vs/">Visual Studio 2022</a> (Community)
- Among Us for desktop, preferrably from the <a href="https://store.steampowered.com/app/945360/Among_Us/">Steam page</a>.

This API tutorial is written by a Windows 10 user, so if anything is different and you find a workaround, add an "if" for your platform.

## S1 - Setup
First, go to the <a href="https://github.com/DillyzThe1/DillyzRoleTemplate/">DillyzRoleTemplate</a> repository & make a new repository from the template.<br>
![GitHub Logo](/docs/assets/create-repo.png)<br>
<br>
Secondly, fill the fields and hit the `Create repository from template` button.<br>
Now, open Github Desktop and hit `Clone repository...` & select the repository, alongside a root folder.<br>
![GitHub Logo](/docs/assets/clone-repo.png)<br>
<br>
Delete the `.template.config` folder and commit it. *Note: No, this will not have to happen in the future after you can start from template.*<br>
Alright, now we need to add an environment variable to an Among Us folder.<br>
<br>
Clone your Among Us game folder under a new name, containing the version of the game you wish.<br>
![GitHub Logo](/docs/assets/clone-among-us.png)<br>
<br>
Now set an environment variable just called `AmongUsModDir` to the folder of the game.<br>
*Example: Setting AmongUsModDir to C:\Program Files (x86)\Steam\steamapps\common\Among Us - Back2Modding*<br>
<br>
Download the version of BepInEx associated with the current version of the mod. <a href="https://builds.bepinex.dev/projects/bepinex_be/577/BepInEx_UnityIL2CPP_x86_ec79ad0_6.0.0-be.577.zip">You can find the latest one for the job here.</a><br>
Put the files inside of the extracted BepInEx zip in your Among Us directory.<br>
If you're on Steam, add a txt called `steam_appid.txt` containing the text `945360`.<br>
This is what your directory should look like:<br>
![GitHub Logo](/docs/assets/among-us-bepinex.png)<br>
<br>
Launch the game and close it immediantly upon seeing the loading screen solve.<br>
Pick up the latest DLL from the <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/releases/latest/">releases page</a> and put it in the `BepInEx/plugins` folder of Among Us.<br>
<br>
Finally, open the folder your project was cloned into and double click the CSProj.<br>
Click on HarmonyMain.cs in Visual Studio & change MOD_NAME, MOD_VERSION, & MOD_ID to your liking. Don't classify it under me.<br>
Change the namespace at the top from `DillyzRoleTemplate` to something like `Top10RolePackage`.<br>
You may also close Visual Studio, rename the .csproj, then boot it up back up with a double click.<br>
Now right clikc the CsProj in Visual Studio & change both the Assembly Name & Default Namespace to `Top10RolePackage` or whatever you had.<br>
At the top of the window of Visual Studio, hit Build -> Build Solution!<br>
<br>
<i><b>Congratulations!!!!</b></i><br>
You've now made a basic among us mod setup that has all dependent DLLS. You may now move on forward.<br>
If you are having a problem so far, please <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/issues/new/">submit an issue on Github</a>.

## S2 - Jester
Now for educational experience, let's make the Jester role in your new mod setup!<br>
First, we'll create our own role. We want it to be a Jester with a purple role color that's independent from Crewmates and Impostors.<br>
Let's start by adding a simple function following the `createRole` function.<br>
Documentation on such: https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/blob/main/docs/DillyzUtil.md#createrole <br>
![GitHub Logo](/docs/assets/jestercode.png)<br>
<br>
Alright, well now we've got the Jester loading in-game with amount and chance options.<br>
![GitHub Logo](/docs/assets/jester-in-game.png)<br>
<br>
But hey, why won't he win on vote-out?<br>
Well, we've got to add that first!<br>
<br>
We'll begin by creating a new class file called PlayerControlPatch.<br>
Hit Ctrl + Shift + A, then type `PlayerControlPatch.cs` and hit `Add`.<br>
<br>
Great, we've got a new empty file now. Let's import `HarmonyLib` and add a patch class within.<br>
In this case, there's a function in the `PlayerControl` class called `Exile`. We'll patch that.<br>
![GitHub Logo](/docs/assets/pcp-embedded-patch.png)<br>
<br>
Great.<br>
Now, we need to make a public static void called Prefix. this is an override function that occurs beofre what we're patching.<br>
If you would like to do it after the function you're patching is called, do Postfix.<br>
<br>
Now let's add the following code to the patched function, recognizing the class we patched's instance.<br>
```cs
public static void Prefix(PlayerControl __instance)
{
	// do stuff here :)
}
```
Wonderful! Now let's add some functionality!<br>
First, I'm going to use the `getRoleName` function in `DillyzUtil` to compare the `__instance` player to a Jester.<br>
I will do so by typing the following inside of my function:<br>
```cs
if (DillyzUtil.getRoleName(__instance) == "Jester") {
	// functionality here
}
```
Yes! This is what we've been waiting for!<br>
Now let's get the `CustomRole` instance and call the `WinGame` function on it, passing `__instance` as the cause!<br>
![GitHub Logo](/docs/assets/pcp-exile-patch.png)<br>
<br>