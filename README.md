# Template
### Server Only Mod
Protect offline players from being sieged.\
When any of the castle owner allies is online, the siege can progress as usual,\
Siege will also progress as usual if the castle is already being sieged/attacked when all the castle owner & allies goes offline.

## Installation
Copy & paste the `CasualSiege.dll` to `\Server\BepInEx\plugins\` folder.

## Removal
Delete the `CasualSiege.dll` from your plugins folder.

## Wetstone Version
1. Uncomment `<!--<DefineConstants>WETSTONE</DefineConstants>-->` in `CasualSiege.csproj`
2. Rebuild the dll.

## Config
<details>
<summary>Config</summary>

- `Enable Mod` [default `true`]\
Enable/disable the mod.
- `Factor in Ally Status` [default `true`]\
Include the player allies online status before blocking siege.
- `Max Ally Cache Age` [default `300`]\
Max age of the player allies cache in seconds.\
If the cache age is older than specified, the cahce will be renewed.\
Don't set this too short as allies gathering process can slightly impact your server performance.\
This cache is only for allies gathering, their online/offline status is updated instantly.

</details>

## More Information
<details>
<summary>Changelog</summary>

`0.0.1`
- Initial Release

</details>

<details>
<summary>Known Issues</summary>

### General
- No known issue.

</details>