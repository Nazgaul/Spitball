using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Login;
using Newtonsoft.Json.Linq;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Cloudents.Jared.Controllers
{
    [Route(".auth/login/custom")]
    public class LoginController : ApiController
    {
        private readonly string m_SigningKey;
        private readonly string m_Audience;
        private readonly string m_Issuer;

        private readonly IZboxWriteService m_ZboxWriteService;

        public LoginController(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
            m_SigningKey = Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY") ?? ConfigurationManager.AppSettings["MS_SigningKey"];
            var website = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") ?? ConfigurationManager.AppSettings["WEBSITE_HOSTNAME"];
            m_Audience = $"https://{website}/";
            m_Issuer = $"https://{website}/";
        }
        //a288bff1-50c7-4290-b6f6-7f8245a335f3
        /*{
    "authenticationToken": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMTk5ODkwIiwidXNlcklkIjoiMTE5OTg5MCIsInZlciI6IjMiLCJpc3MiOiJodHRwczovL3d3dy5zcGl0YmFsbC5jby8iLCJhdWQiOiJodHRwczovL3d3dy5zcGl0YmFsbC5jby8iLCJleHAiOjE1MTE2ODkwNTgsIm5iZiI6MTUwNjUwNTA1OH0.VIc7Hn7EyViTMLZUWd7u7_nXLoaedQFgyflR5-HZMok",
    "user": {
        "userId": "1199890"
    }
}*/
        public IHttpActionResult Post([FromBody] JObject assertion)
        {
            var uniqueId = assertion["hash"].ToObject<Guid>();

            var command = new CreateJaredUserCommand(uniqueId);
            var userIdResult = m_ZboxWriteService.CreateUserJared(command);

            var token = AppServiceLoginHandler.CreateToken(
                new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userIdResult.UserId.ToString()),
                    new Claim(ClaimConst.UserIdClaim,userIdResult.UserId.ToString())
                },
                m_SigningKey,
                m_Audience,
                m_Issuer,
                TimeSpan.FromDays(60));
            return Ok(new LoginResult
            {
                AuthenticationToken = token.RawData,
                User = new LoginResultUser { UserId = userIdResult.UserId.ToString() }
            });
        }
    }
}
