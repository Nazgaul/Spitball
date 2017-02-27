using System;
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
            m_SigningKey = Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY");
            var website = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            m_Audience = $"https://{website}/";
            m_Issuer = $"https://{website}/";
        }

        public IHttpActionResult Post([FromBody] JObject assertion)
        {
            var uniqueId = assertion["hash"].ToObject<Guid>();

            var command = new CreateJaredUserCommand(uniqueId);
            var userIdResult = m_ZboxWriteService.CreateUserJared(command);
            //if (isValidAssertion(assertion)) // user-defined function, checks against a database
            //{
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
