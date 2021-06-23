// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System.IO;
using net.hempux.kabuto.database;
using net.hempux.ninjawebhook.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;

namespace net.hempux.Controllers
{
    [Route("api/notify")]
    [ApiController]
    //[Consumes("application/json")]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly string _appPassword;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        private string _httpMessage;
        private string _teamsChannel;
        private string _serviceUrl;
        private SqliteEngine _sqliteEngine;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            _adapter = adapter;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
            _teamsChannel = configuration["TeamsChannel"] ?? string.Empty;
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
            _appPassword = configuration["MicrosoftAppPassword"] ?? string.Empty;
            _serviceUrl = configuration["MicrosoftServiceURL"] ?? string.Empty;
            _sqliteEngine = new SqliteEngine();
        }


        public virtual IActionResult OnActivity([FromBody] DetailedActivity detailedActivity)
        {


            Console.WriteLine(detailedActivity.DeviceId);
            return StatusCode(501);

            throw new NotImplementedException();
        }
        // Initialization  
        [HttpPost]
        public async Task<IActionResult> Post()
        {

               using (var reader = new StreamReader(Request.Body))
               {
                    _httpMessage = await reader.ReadToEndAsync();
            }

            await CreateChannelConversation(_httpMessage);

               /*
            foreach (var conversationReference in _conversationReferences.Values)
            {
                await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference, BotCallback, default(CancellationToken));
                
            }
            */

            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = "{ \"Status\":\"Messages sent\" }",
                ContentType = "Application/json",
                StatusCode = (int)HttpStatusCode.OK,
            };


        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        {


            switch (_httpMessage.Substring(0,3))
            {
                case "new":
                   // await turnContext.SendNewCardAsync();


                    break;
                case "upd":
                    var _latest = _sqliteEngine.Read();
                    var newActivity = MessageFactory.Text(_latest.Id + " message updated to: " + _httpMessage);
                    newActivity.Id = _latest.Id;
                    //newActivity.Id = _latest.Id;
                    await turnContext.UpdateActivityAsync(newActivity, cancellationToken);
                    break;
                case "del":
                    var _latestmessage = _sqliteEngine.Read();
                    await turnContext.DeleteActivityAsync(_latestmessage.Id, cancellationToken);
                    break;
                case "mul":
                    try
                    {
                        await CreateChannelConversation(_httpMessage);
                        
                    }
                    catch (Exception ex)
                    {
                        throw(ex);
                    }
                    break;
                        default:

                    break;
            }

            
        }

        private async Task CreateChannelConversation(string value)
        {


            var credentials = new MicrosoftAppCredentials(_appId, _appPassword);
            var connector = new ConnectorClient(new Uri(_serviceUrl),credentials);
            var channelData = new Dictionary<string, string>();
            channelData["teamsChannelId"] = _teamsChannel;
            IMessageActivity newMessage = Activity.CreateMessageActivity();
            newMessage.Type = ActivityTypes.Message;
            newMessage.Text = "Hello channel.";
            
            ConversationParameters conversationParams = new ConversationParameters(
                
                isGroup: true,
                bot: null,
                members: null,
                topicName: "New NinjaRMM Issue",
                activity: (Activity)newMessage,
                channelData: channelData);
            var result = await connector.Conversations.CreateConversationAsync(conversationParams);
            Console.WriteLine(result);
        }
    }



}
