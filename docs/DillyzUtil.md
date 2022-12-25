# DillyzUtil
This is the documentation of <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/blob/main/DillyzUtil.cs">DillyzUtil.cs</a>'s functions.

### addButton
Returns <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/blob/main/CustomButton.cs">`CustomButton`</a>.<br>
<br>
<p>
Arguments:
- `Assembly` epicAssemblyFail >> You should pass `System.Reference.Assembly.GetExecutingAssembly()`, as it needs it to load resources.
- `string` name >> The name of your button. By default, it will display this as the buttontext.
- `string` imageName >> The asset name/path of your image. Make sure you set an image as an Embedded Resource and checked startup logs for the id.
- `float` cooldown >> The cooldown of your button in seconds. You should set this to a positive for a flat number & a negative for it to multiply the kill cooldown.
- `bool` isTargetButton >> Determines if your button targets players or gets clicked standalone.
- `string[]` allowedRoles >> A string array of every role allowed to use it. Setting to nothing pretty much means Freeplay only.
- `string[]` roleCantTarget >> Any roles that the button is NOT allowed to target. (Ex: Impostor can't execute `Impostor`s or `ShapeShifter`s.
- `Action<KillButtonCustomData, bool>` onClicked >> A custom return for being clicked. Pass the following code: `delegate(KillButtonCustomData button, bool success) {if (!success) return;}`.
</p>
<br>
Creates a new CustomButton and returns it for modification.

### createRole
Returns <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/blob/main/CustomRole.cs">`CustomRole`</a>.<br>
<br>
<p>
Arguments:
- `string` name >> The name of the role. Displays on reveal, exile, task list, settings, etc.
- `string` subtext >> The subtext/description of the role. Displays on reveal & task list.
- `bool` nameColor >> Determines if the name color is changed on the nametag and meetinghud for the local player.
- `bool` nameColorPublic >> Determines if everyone in the game can see the role color or not.
- `Color32` roleColor >> The color of the role used everywhere. Make a new one with `new Color32(255, 255, 255, 255)`. (Order: RGBA)
- `bool` canSeeTeam >> Determines if your entire team can see your color or not.
- `CustomRoleSide` side >> An enum determining what team you're on. `CustomRoleSide` contains `Impostor`, `Crewmate`, `Independent`, & `LoneWolf`.
- `VentPrivilege` ventPrivilege >> determines how powerful the venting capabilities of the role are. Please only use `Impostor` or `None` for now.
- `bool` canKill >> Determines if the role can use the default kill or not.
- `bool` showEjectText >> Determines if the ejection text should be "was not The Impostor" or "was The Jester".
</p>
<br>
Creates a new CustomRole and returns it for modification.

### getRoleName
Returns `string`.<br>
<br>
<p>
Arguments:
- `PlayerControl` player >> The player you want the role name of.
</p>
<br>
Can return `Crewmate`, `Impostor`, `Scientist`, `Engineer`, `GuardianAngel`, `ShapeShifter`, or your own custom role's name.

### roleSide
Returns <a href="https://github.com/DillyzThe1/DillyzRoleApi-Rewritten/blob/main/CustomRole.cs#L176">`CustomRoleSide`</a>.<br>
<br>
<p>
Arguments:
- `PlayerControl` player >> The player you want the role side of.
</p>
<br>
Will return `Impostor`, `Crewmate`, `Independent`, or `LoneWolf`.

### roleColor
Returns `Color32`.<br>
<br>
<p>
Arguments:
- `PlayerControl` player >> The player you want the role color of.
- `bool` nameText >> Pretty much determines if crewmates are `White` or `CrewmateBlue`.
</p>
<br>
Finds the role color.

### roleText
Returns `string`.<br>
<br>
<p>
Arguments:
- `PlayerControl` player >> The player you want the role text of.
</p>
<br>
Will return the Task Menu text of the player.

### templateRole
Returns `bool`.<br>
<br>
<p>
Arguments:
- `PlayerControl` player >> The player you want to test for a template role.
</p>
<br>
Determines if the player is only an `Impostor` or `Crewmate` according to base game roles.

### findPlayerControl
Returns `PlayerControl`.<br>
<br>
<p>
Arguments:
- `byte` playerId >> The player id you want to find the `PlayerControl` of.
</p>
<br>
Finds the `PlayerControl` by byte id.

### copyColor
Returns `Color32`.<br>
<br>
<p>
Arguments:
- `Color32` ogColor >> The `Color32` you want to copy into a `Color`.
</p>
<br>
Makes a duplicate Color of the Color32.

### convDigitToHexNum
Returns `string`.<br>
<br>
<p>
Arguments:
- `byte` num >> The number to convert.
</p>
<br>
Converts a number to a hexidecimal digit.

### convNumToHexNum
Returns `string`.<br>
<br>
<p>
Arguments:
- `byte` num >> The number to convert.
</p>
<br>
Converts a number into a double digit hexidecimal value.

### colorToHex
Returns `string`.<br>
<br>
<p>
Arguments:
- `Color32` ogColor >> The `Color32` you want to convert.
</p>
<br>
Converts a `Color32` to a hex value.<br>
Example: `new Color32(255, 0, 0, 255)` -> `"#FF0000"`

### getClosestPlayer
Returns `PlayerControl`.<br>
<br>
<p>
Arguments:
- `PlayerControl` centerPlayer >> The player you want to get the closest other player to.
</p>
<br>
Returns the closest player to `centerPlayer`.

### getClosestPlayer
Returns `PlayerControl`.<br>
<br>
<p>
Arguments:
- `PlayerControl` centerPlayer >> The player you want to get the closest other player to.
- `double` mindist >> The distance allowed for the detection.
</p>
<br>
Returns the closest player to `centerPlayer` in the distance of `mindist`.

### getClosestPlayer
Returns `PlayerControl`.<br>
<br>
<p>
Arguments:
- `PlayerControl` centerPlayer >> The player you want to get the closest other player to.
- `System.Collections.Generic.List<String>` roleFilters >> The roles you don't want to target. Set `null` to ignore.
- `double` mindist >> The distance allowed for the detection.
- `bool` shouldBeAlive >> Determines if you wanna target ghosts or living people.
- `bool` canTargetSelf >> Determines if you will target yourself. Will be removed in the future.
</p>
<br>
Returns the closest player to `centerPlayer` in the distance of `mindist`.

### RpcCommitAssassination
<p>
Arguments:
- `PlayerControl` assassinator >> The player commiting the murder.
- `PlayerControl` target >> The player targeted in the murder.
</p>
<br>
Announces to all games to call `commitAssassination` on the 2 players.

### commitAssassination
<p>
Arguments:
- `PlayerControl` assassinator >> The player commiting the murder.
- `PlayerControl` target >> The player targeted in the murder.
</p>
<br>
Does a custom port of a murder, causing `assassinator` to murder `target`.

### DoCustomKill
Returns `IEnumerator`.<br>
<br>
<p>
Arguments:
- `KillAnimation` killanim >> The current KillAnimation instance you wish to use.
- `PlayerControl` assassinator >> The player commiting the murder.
- `PlayerControl` target >> The player targeted in the murder.
</p>
<br>
Does a the actual custom port of a murder, causing `assassinator` to murder `target`.<br>
*(NOTE: DO NOT CALL THIS FUNCTION; USE COMMITASSASSINATION INSTEAD!)*

### getDist
Returns `double`.<br>
<br>
<p>
Arguments:
- `Vector2` p1 >> The first position to use.
- `Vector2` p2 >> The second position to use.
</p>
<br>
Gets the distance between 2 points.

### InFreeplay
Returns `bool`.<br>
<br>
Returns a `bool` based on if the game is in Freeplay or not.

### InGame
Returns `bool`.<br>
<br>
Returns a `bool` based on if the game is in a game or not.

### getSprite
Returns `Sprite`.<br>
<br>
<p>
Arguments:
- `Assembly` assembly >> Your current assembly for texture loading. Pass `System.Reference.Assembly.GetExecutingAssembly()`.
- `string` spritePath >> The path of your asset. Set the image as an Embedded Resource and boot up the game whilst checking the console.
</p>
<br>
Returns a Sprite of the path provided. If no sprite is found, it returns null.

### AddRpcCall
<p>
Arguments:
- `string` rpcName >> The name of your custom RPC.
- `Action<MessageReader>` callback >> The callback activated when the RPC is recieved. Pass `delegate(MessageReader reader) {}`.
</p>
<br>
Adds and RPC call to my custom system and invokes the `callback` when done.

### InvokeRPCCall
<p>
Arguments:
- `string` rpcName >> The name of your custom RPC. Must match an RPC given in AddRpcCall.
- `Action<MessageWriter>` writingCallback >> The callback invoked when data is ready to be written. Pass `delegate(MessageWriter writer) {}`.
</p>
<br>
Gets ready to use RPC in my custom RPC call system and invoked the `writingCallback` when ready to write data.

### color32ToColor
Returns `Color`.<br>
<br>
Arguments:
- `Color32` color32 >> The `Color32` you want to convert.
<br>
Converts a `Color32` to a `Color`.