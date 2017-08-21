using System;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Login;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    public class CustomRegistrationController : ApiController
    {
        private readonly ApplicationUserManager m_UserManager;
        private readonly IZboxWriteService m_ZboxWriteService;

        public CustomRegistrationController(ApplicationUserManager userManager, IZboxWriteService zboxWriteService)
        {
            m_UserManager = userManager;
            m_ZboxWriteService = zboxWriteService;
        }

        // GET api/CustomRegistration
        public async Task<HttpResponseMessage> Post(Register model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var user = new ApplicationUser
            {
                UserName = model.NewEmail,
                Email = model.NewEmail
            };
            var createStatus = await m_UserManager.CreateAsync(user, model.Password);

            if (createStatus.Succeeded)
            {
                CreateUserCommand command = new CreateMembershipUserCommand(Guid.Parse(user.Id),
                    model.NewEmail, model.FirstName, model.LastName,
                    model.Culture, Sex.NotKnown, null);
                var result = await m_ZboxWriteService.CreateUserAsync(command);

                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ClaimConst.UserIdClaim, result.User.Id.ToString()/* User.GetUserId().ToString(CultureInfo.InvariantCulture)*/));
              

                //var identity = await user.GenerateUserIdentityAsync(m_UserManager, result.User.Id, result.UniversityId,
                //     result.UniversityData);

                var claims = identity.Claims.ToList();
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, result.User.Id.ToString(CultureInfo.InvariantCulture)));
                var token = AppServiceLoginHandler.CreateToken(claims,
             Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
            ConfigurationManager.AppSettings["ValidAudience"],
             ConfigurationManager.AppSettings["ValidIssuer"],
            TimeSpan.FromDays(30));

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    User = new
                    {
                        UserId = result.User.Id.ToString(CultureInfo.InvariantCulture)
                    },
                    AuthenticationToken = token.RawData
                });
            }
            var errors = string.Join(" ", createStatus.Errors);
            TraceLog.WriteError(errors);
            return Request.CreateBadRequestResponse(errors);
        }
    }
}
