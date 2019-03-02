# How to Debug the Bot
To debug the bot locally, you need the following things:
* A Discord Bot Token to authenticate against discord. To get a token follow these steps:
  1. Open the [Discord Application Portal](https://discordapp.com/developers/applications/) and create a new app
  2. Switch to **"Bot"** on the left site and click on `create bot`
  3. Under `Token` click on `Copy` to copy your Discord Bot Token. This is the token we need later, so save it to a textfile to use it later!
  4. Now go back to `General information` and copy your `client id`
  5. Navigate to *https://discordapp.com/oauth2/authorize?scope=bot&client_id=CLIENTID* and replace `CLIENTID` with your copied `client id`
  6. Add the bot to your test server
* Create a custom `botconfig.json` file with your token
  1. Create a new `json` file somewhere on your computer and add the following:
        ```
        {
            "DiscordToken": "<your token here>"
        }
        ```
  2. Replaye `your token here` with your previously copied discord token
  3. Save the `json` file and copy the complete path
  4. Create an environment variable called `KP_BOTCONFIG` with the file path as value
