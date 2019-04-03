using Cloudents.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Converters;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Proxy.V1.Service.Session;
using Twilio.Rest.Video.V1;
using Twilio.Rest.Video.V1.Room;
using static Twilio.Rest.Video.V1.CompositionResource;

//using Twilio;
//using Twilio.Rest.Lookups.V1;

namespace Cloudents.Infrastructure.Mail
{
    public class SmsProvider : ISmsProvider, IVideoProvider
    {


        private readonly string[] _badProviders = {
            "Twilio",
            "LEGOS - LOCAL EXCHANGE GLOBAL OPERATION SERVICES",
            "Belgacom Mobile - Proximus",
            "Tismi BV"
        };

        private const string AccountSid = "AC1796f09281da07ec03149db53b55db8d";
        private const string AuthToken = "c4cdf14c4f6ca25c345c3600a72e8b49";

        public SmsProvider()
        {
            TwilioClient.Init(AccountSid, AuthToken);
        }

        public async Task<(string phoneNumber, string country)> ValidateNumberAsync(string phoneNumber, CancellationToken token)
        {
            var result = await Twilio.Rest.Lookups.V1.PhoneNumberResource.FetchAsync(
                pathPhoneNumber: phoneNumber,
                type: new List<string>()
                {
                    "carrier"
                }
            );
            //var uri = new Uri($"https://lookups.twilio.com/v1/PhoneNumbers/{phoneNumber}?Type=carrier");

            //var byteArray = Encoding.ASCII.GetBytes($"{AccountSid}:{AuthToken}");
            //var authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //var headers = new List<KeyValuePair<string, string>>
            //{
            //    new KeyValuePair<string, string>("Authorization", authorization.ToString())
            //};


            //var result = await _restClient.GetAsync<PhoneValidator>(uri, null, headers, token);

            if (result == null)
            {
                return (null, null);
            }
            var carrier = result.Carrier;

            if (carrier.TryGetValue("type", out var carrierType))
            {
                if (!string.Equals(carrierType, "mobile", StringComparison.OrdinalIgnoreCase))
                {
                    return (null, null);
                }
            }
            //https://support.twilio.com/hc/en-us/articles/360004563433-Twilio-Lookups-API-is-Not-Returning-Carrier-Data-for-Canadian-Phone-Numbers
            //if (carrier.Type != null)
            //{
            //    if (!string.Equals(carrier.Type, "mobile", StringComparison.OrdinalIgnoreCase))
            //    {
            //        return null;
            //    }
            //}
            if (carrier.TryGetValue("name", out var carrierName))
            {


                if (_badProviders.Contains(carrierName, StringComparer.OrdinalIgnoreCase))
                {
                    return (null, null);
                }
            }

            return (result.PhoneNumber.ToString(), result.CountryCode);
        }
       

        public async Task CreateRoomAsync(string id,bool needRecord)
        {
            var t = await  RoomResource.CreateAsync(
                uniqueName: id,
                maxParticipants: 2,
                recordParticipantsOnConnect: needRecord);
        }

        public Task CloseRoomAsync(string id)
        {
            return RoomResource.UpdateAsync(id, RoomResource.RoomStatusEnum.Completed);
        }

        private const string ApiKey = "SKa10d29f12eb338d91351795847b35883";
        private const string SecretVideo = "sJBB0TVjomROMH2vj3VwuxvPN9CNHETj";
        public async Task<string> ConnectToRoomAsync(string roomName, string name, bool isTutor)
        {

            var room = await RoomResource.FetchAsync(roomName);
            var grant = new VideoGrant
            {
                Room = room.UniqueName,
            };
            var grants = new HashSet<IGrant> { grant };

           

            // Create an Access Token generator
            var token = new Token(
                AccountSid,
                ApiKey,
                SecretVideo,
                identity: $"{name}_{isTutor}",
                grants: grants);

            return token.ToJwt();

        }


        public async Task ComposeVideo(string roomId)
        {
            
            var room = await RoomResource.FetchAsync(roomId);
            var t = RoomRecordingResource.Read(room.Sid);
            var x = t.Where(s => s.Type == RoomRecordingResource.TypeEnum.Video);
          

            var layout = new
            {
                transcode = new
                {
                    video_sources = new string [] {"MT*"}
                }
            };


            var composition = CompositionResource.Create(
                roomSid: room.Sid,
                audioSources: new List<string>() { "*"},
                videoLayout: layout,
                trim:true,
                //statusCallback: new Uri('http://my.server.org/callbacks'),
                format: FormatEnum.Mp4
            );
        }

       


    }

   
}