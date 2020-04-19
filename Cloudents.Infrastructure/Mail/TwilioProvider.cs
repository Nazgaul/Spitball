using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Newtonsoft.Json;
using Twilio;
using Twilio.Http;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Lookups.V1;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Rest.Video.V1;


namespace Cloudents.Infrastructure.Mail
{
    public class TwilioProvider : IPhoneValidator, IVideoProvider
    {


        private readonly string[] _badProviders =
        {
            "Twilio",
            "LEGOS - LOCAL EXCHANGE GLOBAL OPERATION SERVICES",
            "Belgacom Mobile - Proximus",
            "Tismi BV"
        };

        private const string AccountSid = "AC1796f09281da07ec03149db53b55db8d";
        private const string AuthToken = "c4cdf14c4f6ca25c345c3600a72e8b49";

        static TwilioProvider()
        {
            TwilioClient.Init(AccountSid, AuthToken);
        }

        public static string BuildPhoneNumber(string phoneNumber, string countryCode)
        {
            phoneNumber = Regex.Replace(phoneNumber, "\\([0-9]+?\\)", string.Empty);
            phoneNumber = Regex.Replace(phoneNumber, "[^0-9]", string.Empty);
            phoneNumber = phoneNumber.TrimStart('0');

            countryCode = countryCode.TrimStart('+');
            if (!phoneNumber.StartsWith(countryCode))
            {
                phoneNumber = $"+{countryCode}{phoneNumber}";
            }

            return phoneNumber;
        }

        public async Task<(string? phoneNumber, string? country)> ValidateNumberAsync(string phoneNumber, string countryCode, CancellationToken token)
        {
            phoneNumber = BuildPhoneNumber(phoneNumber, countryCode);

            try
            {
                var result = await PhoneNumberResource.FetchAsync(
                    pathPhoneNumber: phoneNumber,
                    type: new List<string>()
                    {
                        "carrier"
                    }
                );

               

                if (result == null)
                {
                    return (null, null);
                }
                if (result.CountryCode == "CA")
                {
                    return (result.PhoneNumber.ToString(), result.CountryCode);
                }
                var carrier = result.Carrier;

                if (carrier.TryGetValue("type", out var carrierType))
                {
                    if (!string.Equals(carrierType, "mobile", StringComparison.OrdinalIgnoreCase))
                    {
                        return (null, null);
                    }
                }
                if (carrier.TryGetValue("name", out var carrierName))
                {
                    if (_badProviders.Contains(carrierName, StringComparer.OrdinalIgnoreCase))
                    {
                        return (null, null);
                    }
                }

                return (result.PhoneNumber.ToString(), result.CountryCode);
            }
            catch (Twilio.Exceptions.ApiException)
            {
                return (null, null);
            }
        }

        public async Task<(string? phoneNumber, string? country)> ValidateAndSendSmsAsync(string phoneNumber, string countryCode, CancellationToken token)
        {
            // var service = Twilio.Rest.Verify.V2.ServiceResource.Create(friendlyName: "My Verify Service");
            phoneNumber = BuildPhoneNumber(phoneNumber, countryCode);
            //VerificationResource.Create()

            
            var verificationCheck = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel:"sms",
                pathServiceSid: "VA54583e5d91fabd4433a82fc263a8a696"
            );

            return (null, null);
        }


        public async Task CreateRoomAsync(string id, Country country, 
            bool needRecord, Uri callBack, StudyRoomTopologyType studyRoomType)
        {
            var type = RoomResource.RoomTypeEnum.PeerToPeer;
            switch (studyRoomType)
            {
                case StudyRoomTopologyType.SmallGroup:
                    type = RoomResource.RoomTypeEnum.GroupSmall;
                    break;
                case StudyRoomTopologyType.PeerToPeer:
                    type = RoomResource.RoomTypeEnum.PeerToPeer;
                    break;
                case StudyRoomTopologyType.GroupRoom:
                    type = RoomResource.RoomTypeEnum.Group;
                    break;
            }
            //var mediaRegion = "us1";
            //if (country == Country.Israel)
            //{
            //    mediaRegion = "de1";
            //}
            //if (country == Country.India)
            //{
            //    mediaRegion = "us1";
            //}

            await RoomResource.CreateAsync(
                 uniqueName: id,
                 enableTurn: true,
                 //maxParticipants: 2,
                 type: type,
                 statusCallback: callBack,
                 statusCallbackMethod: HttpMethod.Post,
                 recordParticipantsOnConnect: needRecord,
                 mediaRegion: "gll"
            );


        }

        public Task CloseRoomAsync(string id)
        {
            return RoomResource.UpdateAsync(id, RoomResource.RoomStatusEnum.Completed);
        }

        public async Task<bool> GetRoomAvailableAsync(string id)
        {
            var rooms = await RoomResource.ReadAsync(
                uniqueName: id);
            var room = rooms.SingleOrDefault();
            if (room == null)
            {
                return false;
            }
            if (room.Status == RoomResource.RoomStatusEnum.Completed)
            {
                return false;
            }

            return true;

        }

        private const string ApiKey = "SKa10d29f12eb338d91351795847b35883";
        private const string SecretVideo = "sJBB0TVjomROMH2vj3VwuxvPN9CNHETj";
        //public async Task<string?> ConnectToRoomAsync(string roomName, string name)
        //{
        //    try
        //    {
        //        var rooms = await RoomResource.ReadAsync(
        //            status: RoomResource.RoomStatusEnum.InProgress,
        //            uniqueName: roomName);
        //        var room = rooms.FirstOrDefault();
        //        if (room == null)
        //        {
        //            return null;
        //        }
        //        var grant = new VideoGrant
        //        {
        //            Room = room.UniqueName,
        //        };
        //        var grants = new HashSet<IGrant> { grant };

        //        // Create an Access Token generator
        //        var token = new Token(
        //            AccountSid,
        //            ApiKey,
        //            SecretVideo,
        //            identity: name,
        //            grants: grants);

        //        return token.ToJwt();
        //    }
        //    catch (Twilio.Exceptions.ApiException)
        //    {
        //        return null;
        //    }

        //}

        public string CreateRoomToken(string roomName, long userId)
        {
            var grant = new VideoGrant
            {
                Room = roomName,
            };
            var grants = new HashSet<IGrant> { grant };

            // Create an Access Token generator
            var token = new Token(
                AccountSid,
                ApiKey,
                SecretVideo,
                identity: userId.ToString(),
                grants: grants);

            return token.ToJwt();
        }


        //public async Task ComposeVideo(string roomId)
        //{

        //    var room = await RoomResource.FetchAsync(roomId);
        //    var t = RoomRecordingResource.Read(room.Sid);
        //    var x = t.Where(s => s.Type == RoomRecordingResource.TypeEnum.Video);


        //    var layout = new
        //    {
        //        transcode = new
        //        {
        //            video_sources = new string[] { "MT*" }
        //        }
        //    };


        //    var composition = CompositionResource.Create(
        //        roomSid: room.Sid,
        //        audioSources: new List<string>() { "*" },
        //        videoLayout: layout,
        //        trim: true,
        //        //statusCallback: new Uri('http://my.server.org/callbacks'),
        //        format: FormatEnum.Mp4
        //    );
        //}




    }


}