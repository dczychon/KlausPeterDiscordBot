# Bot configuration

The bot allowes you to load it´s configuration via a `.json` file. This file has the following parameters:

## Configuration file refference
|Name|Type|Description|
|-|-|-|
|DiscordToken|`string`|Discord Bot token to authenticate against Discord|
|FortniteTrackerToken|`string`|Fortinte Tracker API Token required for the `!fortinte-stats` command. You can get your token here: [Fortinte Tracker API](https://fortnitetracker.com/site-api)
|EnableDebugCommandsForAdmins|`bool`|If debug commands should be available for admins if the bot is running
|RespondeOnlyToAdminsInPrivateChannels|`bool`|If the bot should only answer messages in pirvate dm channels from bot admins
|RespondeInChannels|`array of 64-Bit integers`|Discord channel ID´s in wich the bot should responde. If this is empty, the bot doesn´t answer in any public channels!
|RespondeToUsers|`array of 64-Bit integers`|Discord user ID´s of users the bot should anser to in public channels. If this is empty, the bot answers every user in public channels
BotAdministrators|`array of 64-Bit integers`|Discord user ID´s of users who administrate the bot. Those users can run admin only commands.
Here an example of the configuration file content:
```
{
  "DiscordToken": "ileuwarhgiaerugiuregb",
  "FortniteTrackerToken": "iezurgberg-wef34-eg4-greggr",
  "EnableDebugCommandsForAdmins": true,
  "RespondeOnlyToAdminsInPrivateChannels": false,
  "RespondeInChannels": [
    131546846315468442,
    646431131946664441
  ],
  "RespondeToUsers": [],
  "BotAdministrators": [
    111112222233333366
  ]
}
```
<br>

## Load the configuration file
Thera are several ways to load the `.json` configration file:
1. Place a file called `botconfig.json` in the same directory as the bot (where the Bots .dll file is located). The Bot will load this file on startup with no fourther action required.
2. Create an environment variable called **`KP_BOTCONFIG`** with the full file location (filename included) as value. The bot will look for this environment variable on startup and will load the given file. If the environment variable is set, the `botconfig.json` file in the bot directory will be ignored.