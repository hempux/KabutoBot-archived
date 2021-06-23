using Microsoft.Bot.Schema;
using net.hempux.ninjawebhook.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace net.hempux.kabuto
{

public static class Cardmanager {


        public static Attachment CreateNinjaInfoCard(DetailedActivity activity)
        {
            NinjaApi ninjaApi = new NinjaApi();
            // combine path for cross platform support
            var basepath = Directory.GetCurrentDirectory();
            var cardjson = Path.Combine(basepath,"cards","ninjainfocard.json");
            var adaptiveCardJson = File.ReadAllText(cardjson);

            DateTimeOffset triggeredTimestamp = DateTimeOffset.FromUnixTimeSeconds(activity.ActivityTime);

            var devicedata = ninjaApi.getDevice(activity.DeviceId);
            JObject devicedataJson = JObject.Parse(devicedata);
            // string orgName = _sqlite.lookupg((string)devicedataJson["customer_id"]

            var ninjacard = adaptiveCardJson.Replace("PH_MESSAGE", activity.Message)
                                            .Replace("PH_STATUS", activity.Status)
                                            .Replace("PH_DATETIMENOW", triggeredTimestamp.ToString())
                                            .Replace("PH_DISPLAY_NAME",(string)devicedataJson["display_name"])
                                            .Replace("PH_COMPUTER_NAME",(string)devicedataJson["dns_name"])
                                             //.Replace("PH_ORG_NAME", orgName)
                                            ;
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(ninjacard),
            };
            return adaptiveCardAttachment;
        }



        public static Attachment WakeupCard()
        {
            // combine path for cross platform support
            var basepath = Directory.GetCurrentDirectory();
            var cardjson = Path.Combine(basepath, "cards", "ninjainfocard.json");
            var adaptiveCardJson = File.ReadAllText(cardjson);
            
            var adaptiveCardAttachment = new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(adaptiveCardJson),
                };
            return adaptiveCardAttachment;
            
        }

    }
}