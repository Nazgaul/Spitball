using System;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Login;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using System.Linq;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    public class CustomLoginController : ApiController
    {
        //  public ApiServices Services { get; set; }
        // public IServiceTokenHandler Handler { get; set; }

        private readonly IZboxReadService m_ZboxReadService;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IFacebookService m_FacebookService;
        private readonly IGoogleService m_GoogleService;
        private readonly ApplicationUserManager m_UserManager;

        public CustomLoginController(IZboxReadService zboxReadService, IZboxWriteService zboxWriteService, IFacebookService facebookService, IGoogleService googleService, ApplicationUserManager userManager)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_FacebookService = facebookService;
            m_GoogleService = googleService;
            m_UserManager = userManager;
        }

        // GET api/CustomLogin
        public async Task<HttpResponseMessage> Post(LogInRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            try
            {
                var query = new GetUserByEmailQuery(loginRequest.Email);
                var user = await m_UserManager.FindByEmailAsync(loginRequest.Email);
                var systemUser = await  m_ZboxReadService.GetUserDetailsByEmailAsync(query);
                var logIn = await m_UserManager.CheckPasswordAsync(user, loginRequest.Password);
                if (systemUser == null)
                {
                    return Request.CreateBadRequestResponse();
                }
                if (logIn)
                {
                    var identity = await user.GenerateUserIdentityAsync(m_UserManager, systemUser.Id, systemUser.UniversityId,
                         systemUser.UniversityData);
                    var claims = identity.Claims.ToList();
                    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, systemUser.Id.ToString(CultureInfo.InvariantCulture)));
                    var token = AppServiceLoginHandler.CreateToken(claims,
                 GetEnvironmentVariableAuth(),
                ConfigurationManager.AppSettings["ValidAudience"],
                 ConfigurationManager.AppSettings["ValidIssuer"],
                TimeSpan.FromDays(30));

                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        User = new
                        {
                            UserId = systemUser.Id.ToString(CultureInfo.InvariantCulture)
                        },
                        AuthenticationToken = token.RawData
                    });
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"LogOn model : {loginRequest} ", ex);
            }
            return Request.CreateBadRequestResponse();
        }

        public static string GetEnvironmentVariableAuth()
        {
            return Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY") ??
                   Convert.ToBase64String(
                       System.Text.Encoding.UTF8.GetBytes(
                           "947726dd6318753268f3bfbe5e87ae2afe220db399c26e119c181a59227b0c60"));
        }

        [HttpPost]
        [Route("api/facebookLogin")]
        public async Task<HttpResponseMessage> FacebookLoginAsync(ExternalLoginRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            var facebookUserData = await m_FacebookService.FacebookLogOnAsync(model.AuthToken);
            if (facebookUserData == null)
            {
                return Request.CreateBadRequestResponse("auth token is invalid");
            }
            try
            {
                var query = new GetUserByFacebookQuery(facebookUserData.Id);
                var user = await m_ZboxReadService.GetUserDetailsByFacebookIdAsync(query);
                if (user == null)
                {
                    var command = new CreateFacebookUserCommand(facebookUserData.Id, facebookUserData.Email,
                         facebookUserData.LargeImage,
                        facebookUserData.First_name,
                        facebookUserData.Last_name,
                        facebookUserData.Locale, facebookUserData.SpitballGender, null);
                    var commandResult = await m_ZboxWriteService.CreateUserAsync(command);
                    user = new LogInUserDto
                    {
                        Id = commandResult.User.Id,
                        Culture = commandResult.User.Culture,
                        Image = facebookUserData.LargeImage,
                        Name = facebookUserData.Name,
                        UniversityId = commandResult.UniversityId,
                        UniversityData = commandResult.UniversityData,
                        Score = commandResult.User.Score
                    };
                }

                var identity = ApplicationUser.GenerateUserIdentity(user.Id, user.UniversityId, user.UniversityData);

                var claims = identity.Claims.ToList();
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)));
                var token = AppServiceLoginHandler.CreateToken(claims,
             Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
            ConfigurationManager.AppSettings["ValidAudience"],
             ConfigurationManager.AppSettings["ValidIssuer"],
            TimeSpan.FromDays(30));

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    User = new
                    {
                        UserId = user.Id.ToString(CultureInfo.InvariantCulture)
                    },
                    AuthenticationToken = token.RawData
                });
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Error in facebook login - facebook data - {0} authkey {2} ex {1} ",
                    facebookUserData, ex, model.AuthToken));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("api/googleLogin"), ActionName("GoogleLogin")]
        public async Task<HttpResponseMessage> GoogleLoginAsync(ExternalLoginRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            var googleUserData = await m_GoogleService.GoogleLogOnAsync(model.AuthToken);
            if (googleUserData == null)
            {
                return Request.CreateBadRequestResponse("auth token is invalid");
            }
            if (string.IsNullOrEmpty(googleUserData.Email))
            {
                return Request.CreateBadRequestResponse("email is required");
            }
            try
            {
                var query = new GetUserByEmailQuery(googleUserData.Email);
                var user = await m_ZboxReadService.GetUserDetailsByEmailAsync(query);
                if (user == null)
                {
                    var command = new CreateGoogleUserCommand(googleUserData.Email,
                        googleUserData.Id,
                        googleUserData.Picture,

                        googleUserData.FirstName,
                        googleUserData.LastName,
                        googleUserData.Locale, Sex.NotKnown, null);
                    var commandResult = await m_ZboxWriteService.CreateUserAsync(command);
                    user = new LogInUserDto
                    {
                        Id = commandResult.User.Id,
                        Culture = commandResult.User.Culture,
                        Image = googleUserData.Picture,
                        Name = googleUserData.Name,
                        UniversityId = commandResult.UniversityId,
                        UniversityData = commandResult.UniversityData,
                        Score = commandResult.User.Score
                    };
                }

                var identity = ApplicationUser.GenerateUserIdentity(user.Id, user.UniversityId, user.UniversityData);

                var claims = identity.Claims.ToList();
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(CultureInfo.InvariantCulture)));
                var token = AppServiceLoginHandler.CreateToken(claims,
             Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
            ConfigurationManager.AppSettings["ValidAudience"],
             ConfigurationManager.AppSettings["ValidIssuer"],
            TimeSpan.FromDays(30));

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    User = new
                    {
                        UserId = user.Id.ToString(CultureInfo.InvariantCulture)
                    },
                    AuthenticationToken = token.RawData
                });
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Error in google login - google data - {0} authkey {2} ex {1} ", googleUserData, ex, model.AuthToken));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
