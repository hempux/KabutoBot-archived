# Kabuto aka NinjaRmm webhook-bridge
![Screenshot of sample card](docs/Sample_adaptivecard.png?raw=true)



## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 5.0

  ```bash
  # determine dotnet version
  dotnet --version
  ```
### Updating models
The ninjaRMM API webhook stubs are generated from openAPI/swagger using [`OpenAPI-Generator`](https://github.com/OpenAPITools/openapi-generator#13---download-jar) with definitions from https://resources.ninjarmm.com/API/webhook.yaml with documentation available [here](https://eu.ninjarmm.com/apidocs/)

```bash
# Generate server code from openAPI code
java -jar openapi-generator-cli.jar generate -i https://resources.ninjarmm.com/API/webhook.yaml -g aspnetcore -o ninjawebhook --additional-properties packageName=net.hempux.ninjawebhook,buildTarget=library
```
After the code is generated you can move it from the ninjawebhook/src folder.

## To run the bot

- Run the bot from a terminal or from Visual Studio:

  ```bash
  # run the bot
  dotnet run
  ```


## Testing the bot using Bot Framework Emulator

[Bot Framework Emulator](https://github.com/microsoft/botframework-emulator) is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the latest Bot Framework Emulator from [here](https://github.com/Microsoft/BotFramework-Emulator/releases)

### Connect to the bot using Bot Framework Emulator

**For this to work you will either have to leave `MicrosfotBotAppId` and `MicrosoftAppPassword` blank, or supply those values while connecting with the emulator, otherwise you will recieve a "401 Unauthorized" error.**


- Launch Bot Framework Emulator
- File -> Open Bot
- Enter a Bot URL of `http://localhost:3978/api/messages`

With the Bot Framework Emulator connected to your running bot, the sample will not respond to an HTTP GET that will trigger a proactive message.  The proactive message can be triggered from the command line using `curl`, `powershell` or similar tooling.

### Using curl/powershell

- Send a get request to `http://localhost:3978/api/notify` to proactively message users from the bot.

   ```bash
    curl -X POST --data-binary samplerequests\WebhookDeviceMessage.json http://localhost:3978/api/notify
   ```

   ```powershell
   Invoke-RestMethod -Uri http://localhost:3978/api/ninjawebhook -Body (Get-Content .\samplerequests\WebhookDeviceMessage.json) -Method Post -ContentType "application/json"
   ```

- Using the Bot Framework Emulator, notice a message was sent to the user from the bot.

## Setting up the  bot.

Visit the [Bot Framework developer portal](https://dev.botframework.com/bots/new) to register a new bot so that teams knows how to communicate with your bot.

To set up the bot and select what channel the ninjaRMM messages will be sent to you'll have to configure `TeamsChannel` and `MicrosoftServiceURL` in the file `appsettings.json`

Start the bot and @Mention it in the channel you want to use with the message `channelinfo` to get the correct strings.

![channelinfo command screenshot](docs/channelinfo_command.png?raw=true)

# Troubleshooting
`AADSTS700016`  Application with identifier '\<your Micrcrosoft App ID>\' was not found in the directory 'botframework.com'
* Sign into the azure portal and go to the [App registrations](https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade) and click `Authentication` in the left side panel, verify that the `Supported account types` is set to `Accounts in any organizational directory (Any Azure AD directory - Multitenant)` so that the bot framework can authenticate your bot.

`AADSTS7000215`
* Check that `appsettings.json` has the correct `MicrosoftAppPassword`

## Enabling webhooks from NinjaRMM

Visit the [APIdocs](https://eu.ninjarmm.com/apidocs/?links.active=core#/management/configureWebhook) page and send a HTTP POST message to the appropriate endpoint to enable the webhook message to your bot. The URL to send in the POST request should be`https://<yourbotadress>/api/ninjawebhook`


## Deploy this bot to Azure

To learn more about deploying a bot to Azure, see [Deploy your bot to Azure](https://aka.ms/azuredeployment) for a complete list of deployment instructions.

## Further reading

- [Bot Framework Documentation](https://docs.botframework.com)
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Send proactive messages](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-proactive-message?view=azure-bot-service-4.0&tabs=js)
- [continueConversation Method](https://docs.microsoft.com/en-us/javascript/api/botbuilder/botframeworkadapter#continueconversation)
- [getConversationReference Method](https://docs.microsoft.com/en-us/javascript/api/botbuilder-core/turncontext#getconversationreference)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [.NET Core CLI tools](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
