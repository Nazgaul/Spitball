using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.StudyRooms;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class TwilioVideoWebHook
    {
        /// <summary>
        /// https://www.twilio.com/docs/video/api/status-callbacks
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("TwilioVideoWebHook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "roomCallback")] HttpRequest req,
            [Inject] ICommandBus commandBus,
            ILogger log,

            CancellationToken token)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var client = new TelemetryClient();
            foreach (var (key, value) in req.Form)
            {
                client.TrackTrace($"{key}, {value}");
            }

            var request = new TwilioWebHookRequest(req.Form);
            var id = Guid.Parse(req.Query["id"]);
            client.Context.Session.Id = id.ToString();
            client.TrackEvent($"Room Status {id}",
                request.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToDictionary
                (
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(request, null)?.ToString()

                ));
            if (request.RoomStatus == "completed")
            {
                var command = new EndStudyRoomSessionTwilioCommand(id, request.RoomName);
                await commandBus.DispatchAsync(command, token);
            }

            //if (request.StatusCallbackEvent.Equals("participant-disconnected", StringComparison.OrdinalIgnoreCase))
            //{
            //    var command = new StudyRoomSessionParticipantDisconnectedCommand(id);

            //    await _commandBus.DispatchAsync(command, token);

            //}
            //else if (request.StatusCallbackEvent.Equals("participant-connected", StringComparison.OrdinalIgnoreCase))
            //{
            //    var command = new StudyRoomSessionParticipantReconnectedCommand(id);
            //    await _commandBus.DispatchAsync(command, token);
            //}

            return new OkResult();

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //return new OkObjectResult(responseMessage);
        }


        public class TwilioWebHookRequest
        {
            public TwilioWebHookRequest(IFormCollection form)
            {
                AccountSid = form["AccountSid"];
                RoomName = form["RoomName"];
                RoomSid = form["RoomSid"];
                RoomStatus = form["RoomStatus"];
                RoomType = form["RoomType"];
                StatusCallbackEvent = form["StatusCallbackEvent"];
                Timestamp = DateTime.Parse(form["Timestamp"], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                ParticipantSid = form["ParticipantSid"];
                ParticipantStatus = form["ParticipantStatus"];
                ParticipantDuration = form["ParticipantDuration"];
                ParticipantIdentity = form["ParticipantIdentity"];
                if (form["RoomDuration"].ToString() != null)
                {
                    RoomDurationInSeconds = int.Parse(form["RoomDuration"]);
                }

                TrackSid = form["TrackSid"];
                TackKind = form["TrackKind"];

            }

            public StringValues TackKind { get; set; }

            public string AccountSid { get; set; }
            public string RoomName { get; set; }
            public string RoomSid { get; set; }
            public string RoomStatus { get; set; }
            public string RoomType { get; set; }
            public string StatusCallbackEvent { get; set; }
            public DateTime Timestamp { get; set; }
            public string ParticipantSid { get; set; }
            public string ParticipantStatus { get; set; }
            public string ParticipantDuration { get; set; }
            public string ParticipantIdentity { get; set; }
            public int RoomDurationInSeconds { get; set; }
            public string TrackSid { get; set; }
        }
    }
}
