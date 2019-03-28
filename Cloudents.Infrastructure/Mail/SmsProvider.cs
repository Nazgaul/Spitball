using Cloudents.Core.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;

//using Twilio;
//using Twilio.Rest.Lookups.V1;

namespace Cloudents.Infrastructure.Mail
{
    public class SmsProvider : ISmsProvider , IVideoProvider
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




        public class PhoneValidator
        {
            //public object caller_name { get; set; }
            //public string country_code { get; set; }
            [JsonProperty("phone_number")]
            public string PhoneNumber { get; set; }
            //public string national_format { get; set; }
            [JsonProperty("carrier")]
            public Carrier Carrier { get; set; }
            //public object add_ons { get; set; }
            //public string url { get; set; }
        }

        public class Carrier
        {
            //public string mobile_country_code { get; set; }
            //public string mobile_network_code { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            //public object error_code { get; set; }
        }


        public  Task CreateRoomAsync(string id)
        {
            return RoomResource.CreateAsync(
                uniqueName: id,
                maxParticipants: 2,
                recordParticipantsOnConnect: true);
        }

        public Task CloseRoomAsync(string id)
        {
            return RoomResource.UpdateAsync(id, RoomResource.RoomStatusEnum.Completed);
        }

        private const string ApiKey = "SKa10d29f12eb338d91351795847b35883";
        private const string SecretVideo = "sJBB0TVjomROMH2vj3VwuxvPN9CNHETj";
        public async Task<string> ConnectToRoomAsync(string roomName, string name)
        {
      
        var room = await RoomResource.FetchAsync(roomName);

            var grant = new VideoGrant
            {
                Room = room.UniqueName,
            };
            var grants = new HashSet<IGrant> { grant };

           // var name = identityName;
            if (string.IsNullOrEmpty(name))
            {
                name = GetName();
            }

            // Create an Access Token generator
            var token = new Token(
                AccountSid,
                ApiKey,
                SecretVideo,
                identity: name,
                grants: grants);

            return token.ToJwt();

        }


        #region Borrowed from https://github.com/twilio/video-quickstart-js/blob/1.x/server/randomname.js

        readonly string[] _adjectives =
        {
            "Abrasive", "Brash", "Callous", "Daft", "Eccentric", "Feisty", "Golden",
            "Holy", "Ignominious", "Luscious", "Mushy", "Nasty",
            "OldSchool", "Pompous", "Quiet", "Rowdy", "Sneaky", "Tawdry",
            "Unique", "Vivacious", "Wicked", "Xenophobic", "Yawning", "Zesty"
        };

        readonly string[] _firstNames =
        {
            "Anna", "Bobby", "Cameron", "Danny", "Emmett", "Frida", "Gracie", "Hannah",
            "Isaac", "Jenova", "Kendra", "Lando", "Mufasa", "Nate", "Owen", "Penny",
            "Quincy", "Roddy", "Samantha", "Tammy", "Ulysses", "Victoria", "Wendy",
            "Xander", "Yolanda", "Zelda"
        };

        readonly string[] _lastNames =
        {
            "Anchorage", "Berlin", "Cucamonga", "Davenport", "Essex", "Fresno",
            "Gunsight", "Hanover", "Indianapolis", "Jamestown", "Kane", "Liberty",
            "Minneapolis", "Nevis", "Oakland", "Portland", "Quantico", "Raleigh",
            "SaintPaul", "Tulsa", "Utica", "Vail", "Warsaw", "XiaoJin", "Yale",
            "Zimmerman"
        };

      

        string GetName() => $"{_adjectives.Random()} {_firstNames.Random()} {_lastNames.Random()}";

        #endregion

      
    }

    static class StringArrayExtensions
    {
        static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        internal static string Random(this IReadOnlyList<string> array) => array[_random.Next(array.Count)];
    }
}