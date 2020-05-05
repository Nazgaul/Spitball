using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Command.StudyRooms;
using Cloudents.Infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class TwilioVideoWebHook
    {
        /// <summary>
        /// https://www.twilio.com/docs/video/api/status-callbacks
        /// </summary>
        /// <param name="req"></param>
        /// <param name="commandBus"></param>
        /// <param name="log"></param>
        /// <param name="token"></param>
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
            var roomId = Guid.Parse(req.Query["id"]);
            client.Context.Session.Id = roomId.ToString();
            client.TrackEvent($"Room Status {roomId}",
                request.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToDictionary
                (
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(request, null)?.ToString()

                ));
            if (string.Equals(request.StatusCallbackEvent, "room-ended", StringComparison.OrdinalIgnoreCase))
            {
                var command = new EndStudyRoomSessionTwilioCommand(roomId, request.SessionId);
                await commandBus.DispatchAsync(command, token);
            }
         
            if (request.StatusCallbackEvent.Equals("participant-connected", StringComparison.OrdinalIgnoreCase))
            {
                var command = new StudyRoomSessionUserConnectedCommand(roomId, request.SessionId, request.UserId);
                await commandBus.DispatchAsync(command, token);
            }

            if (request.StatusCallbackEvent.Equals("participant-disconnected", StringComparison.OrdinalIgnoreCase))
            {
                if (!request.ParticipantDuration.HasValue)
                {
                    throw new ArgumentException("need to have duration");
                }
                var command = new StudyRoomSessionUserDisconnectedCommand(roomId, request.SessionId, request.UserId, request.ParticipantDuration.Value);

                await commandBus.DispatchAsync(command, token);

            }

            return new OkResult();

           
        }


        public class TwilioWebHookRequest
        {
            public TwilioWebHookRequest(IFormCollection form)
            {
               // AccountSid = form["AccountSid"];
                SessionId = form["RoomName"];
               // RoomSid = form["RoomSid"];
               // RoomStatus = form["RoomStatus"];
               // RoomType = form["RoomType"];
                StatusCallbackEvent = form["StatusCallbackEvent"];
               // Timestamp = DateTime.Parse(form["Timestamp"], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
               // ParticipantSid = form["ParticipantSid"];
               // ParticipantStatus = form["ParticipantStatus"];
                if (!string.IsNullOrEmpty(form["ParticipantDuration"]))
                {
                    ParticipantDuration = TimeSpan.FromSeconds(double.Parse(form["ParticipantDuration"]));
                }

                if (!string.IsNullOrEmpty(form["ParticipantIdentity"]))
                {
                    var (userId, name) = TwilioProvider.ParseIdentity(form["ParticipantIdentity"]);
                    UserId = userId;
                }

                //if (!string.IsNullOrEmpty(form["RoomDuration"]))
                //{
                //    RoomDurationInSeconds = int.Parse(form["RoomDuration"]);
                //}

               // TrackSid = form["TrackSid"];
               // TackKind = form["TrackKind"];

            }

           // public StringValues TackKind { get;  }

          //  public string AccountSid { get;  }
            public string SessionId { get;  }
          //  public string RoomSid { get;  }
          //  public string RoomStatus { get;  }
          //  public string RoomType { get;  }
            public string StatusCallbackEvent { get;  }
          //  public DateTime Timestamp { get;  }
           // public string ParticipantSid { get;  }
           // public string ParticipantStatus { get;  }
            public TimeSpan? ParticipantDuration { get;  }
            public long UserId { get;  }
            //public int? RoomDurationInSeconds { get;  }
           // public string TrackSid { get; }
        }
    }
}
