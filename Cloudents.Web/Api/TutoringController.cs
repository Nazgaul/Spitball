using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TutoringController : ControllerBase
    {


        private const string AccountSid = "AC1796f09281da07ec03149db53b55db8d";
        private const string AccountSecret = "c4cdf14c4f6ca25c345c3600a72e8b49";
        private const string SecretVideo = "sJBB0TVjomROMH2vj3VwuxvPN9CNHETj";
        private const string ApiKey = "SKa10d29f12eb338d91351795847b35883";


        static TutoringController()
        {
            TwilioClient.Init(AccountSid, AccountSecret);
        }


        /// <summary>
        /// Generate room
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        public IActionResult CreateAsync()
        {
            var room = RoomResource.Create(uniqueName: Guid.NewGuid().ToString());
            return Ok(new
            {
                name = room.UniqueName
            });
        }


        [HttpGet("join")]
        public async Task<IActionResult> ConnectAsync(string roomName)
        {

            var room = await RoomResource.FetchAsync(roomName);
            var grant = new VideoGrant
            {
                Room = room.UniqueName
            };

            var grants = new HashSet<IGrant> { grant };

            // Create an Access Token generator
            var token = new Token(
                AccountSid,
                ApiKey,
                SecretVideo,
                identity: GetName(),
                grants: grants);


            return Ok(new
            {
                token = token.ToJwt()

            }
            );
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