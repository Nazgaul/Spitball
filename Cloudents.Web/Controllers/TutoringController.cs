using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;

namespace Cloudents.Web.Controllers
{
    public class TutoringController : Controller
    {
        private const string AccountSid = "AC1796f09281da07ec03149db53b55db8d";
        private const string AccountSecret = "c4cdf14c4f6ca25c345c3600a72e8b49";
        private const string SecretVideo = "sJBB0TVjomROMH2vj3VwuxvPN9CNHETj";
        private const string ApiKey = "SKa10d29f12eb338d91351795847b35883";


        static TutoringController()
        {
            TwilioClient.Init(AccountSid, AccountSecret);
        }
        // GET
        public IActionResult Index(string roomName)
        {
            var room = RoomResource.Create(uniqueName: roomName);
            //var grant = new VideoGrant();
            //grant.Room = room.UniqueName;

            var identity = "example-user";
            //var token = new Token(AccountSid, ApiKey,SecretVideo, identity);



            //const string identity = "user";

            // Create a Video grant for this token
            var grant = new VideoGrant();
            grant.Room = room.UniqueName;

            var grants = new HashSet<IGrant> { grant };

            // Create an Access Token generator
            var token = new Token(
                AccountSid,
                ApiKey,
                SecretVideo,
                identity: identity,
                grants: grants);



            ViewBag.room = token.ToJwt();
            return View();
        }
    }
}