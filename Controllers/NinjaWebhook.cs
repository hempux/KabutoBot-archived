// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using net.hempux.kabuto;
using net.hempux.kabuto.database;
using Newtonsoft.Json;
using net.hempux.ninjawebhook.Models;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Connector;
using Serilog;
using net.hempux.Utilities;

namespace net.hempux.Controllers
{
    [Route("api/ninjawebhook")]
    [ApiController]
    [Consumes("application/json")]
    public class NinjaApiController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly string _appPassword;
        private readonly string _serviceUrl;
        private readonly string _teamsChannel;
        private SqliteEngine _sqliteEngine;


        public NinjaApiController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration)
        {

            _adapter = adapter;
            _sqliteEngine = new SqliteEngine();
            // Credentials
            _appId = configuration["MicrosoftAppId"] ?? string.Empty;
            _appPassword = configuration["MicrosoftAppPassword"] ?? string.Empty;
            // Teams settings
            _teamsChannel = AppSettings.Current.Teams["TeamsChannel"] ?? string.Empty;
            _serviceUrl = AppSettings.Current.Teams["MicrosoftServiceURL"] ?? string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> OnActivity([FromBody] DetailedActivity detailedActivity)
        {
            await CreateChannelConversation(detailedActivity);
            _sqliteEngine.createDashboardIssue(detailedActivity);
            Log.Information("New NinjaRMM activity received (acitivty ID: {id} )", detailedActivity.Id);
            
            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(detailedActivity),
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK,
            };

        }

        [HttpGet]
        public IActionResult Get()
        {
            return StatusCode(501);

        }

        private async Task CreateChannelConversation(DetailedActivity activity)
        {
            var credentials = new MicrosoftAppCredentials(_appId, _appPassword);
            var connector = new ConnectorClient(new Uri(_serviceUrl), credentials);
            var channelData = new Dictionary<string, string>();
            channelData["teamsChannelId"] = _teamsChannel;
            IMessageActivity newMessage = Activity.CreateMessageActivity();
            newMessage.Type = ActivityTypes.Message;
            newMessage.Attachments.Add(Cardmanager.CreateNinjaInfoCard(activity));
            ConversationParameters conversationParams = new ConversationParameters(

                isGroup: true,
                bot: null,
                members: null,
                topicName: "New NinjaRMM Issue",
                activity: (Activity)newMessage,
                channelData: channelData);
            var result = await connector.Conversations.CreateConversationAsync(conversationParams);
            //_sqliteEngine.Insert(new ActivityModel { Id = result.Id,Text = result.ActivityId }); // id = messageid that can be used to update card, ActivityID = "channelID and messageID"
        }
    }

}




    



