﻿using System;
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
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [Authorize]
    [MobileAppController]
    public class AccountController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly ApplicationUserManager m_UserManager;
        private readonly IQueueProvider m_QueueProvider;

        public AccountController(IZboxCacheReadService zboxReadService, IZboxWriteService zboxWriteService, ApplicationUserManager userManager, IQueueProvider queueProvider)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_UserManager = userManager;
            m_QueueProvider = queueProvider;
        }

        //public IServiceTokenHandler Handler { get; set; }


        // GET api/Account
        [HttpGet]
        [Route("api/account/details")]
        public async Task<HttpResponseMessage> Details()
        {
            var retVal = await m_ZboxReadService.GetUserDataAsync(new GetUserDetailsQuery(User.GetCloudentsUserId()));
            return Request.CreateResponse(new
            {
                retVal.Id,
                retVal.UniversityId,
                retVal.Name,
                retVal.Image,
                retVal.IsAdmin,
                //retVal.FirstTimeDashboard,
                //retVal.Score,
                retVal.UniversityCountry,
                retVal.UniversityName,
                tokenValid = retVal.UniversityId == User.GetUniversityId()
            });
        }

        [HttpPost]
        [Route("api/account/tokenrefresh")]
        public async Task<HttpResponseMessage> Refresh()
        {

            var systemData = await m_ZboxReadService.GetUserDetailsById(new GetUserByIdQuery(User.GetCloudentsUserId()));

            if (!systemData.UniversityId.HasValue)
            {
                return Request.CreateBadRequestResponse("user don't have university");
            }

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimConst.UserIdClaim, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));


            identity.AddClaim(new Claim(ClaimConst.UniversityIdClaim,
                    systemData.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));

            identity.AddClaim(new Claim(ClaimConst.UniversityDataClaim,
                    systemData.UniversityData.HasValue ?
                    systemData.UniversityData.Value.ToString(CultureInfo.InvariantCulture)
                    : systemData.UniversityId.Value.ToString(CultureInfo.InvariantCulture)));

            var claims = identity.Claims.ToList();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));
            var token = AppServiceLoginHandler.CreateToken(claims,
         Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
        ConfigurationManager.AppSettings["ValidAudience"],
         ConfigurationManager.AppSettings["ValidIssuer"],
        TimeSpan.FromDays(30));

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                User = new
                {
                    UserId = User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)
                },
                AuthenticationToken = token.RawData
            });
        }


        [HttpPost, Route("api/account/university/create")]
        public HttpResponseMessage CreateUniversity(CreateUniversityRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new CreateUniversityCommand(model.Name, model.Country, User.GetCloudentsUserId());
            m_ZboxWriteService.CreateUniversity(command);

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimConst.UserIdClaim, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));
            identity.AddClaim(new Claim(ClaimConst.UniversityIdClaim,
                    command.Id.ToString(CultureInfo.InvariantCulture)));

            identity.AddClaim(new Claim(ClaimConst.UniversityDataClaim,
                    command.Id.ToString(CultureInfo.InvariantCulture)));

            var claims = identity.Claims.ToList();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));
            var token = AppServiceLoginHandler.CreateToken(claims,
         Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
        ConfigurationManager.AppSettings["ValidAudience"],
         ConfigurationManager.AppSettings["ValidIssuer"],
        TimeSpan.FromDays(30));

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                User = new
                {
                    UserId = User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)
                },
                AuthenticationToken = token.RawData
            });



        }



        [HttpPost]
        //[VersionedRoute("api/account/university", 2)]
        [Route("api/account/university")]
        public async Task<HttpResponseMessage> UpdateUniversity(UpdateUniversityRequest model)
        {

            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            model.StudentId = model.StudentId ?? model.Code; //due to ios bug
            var needId = await m_ZboxReadService.GetUniversityNeedIdAsync(model.UniversityId);
            if (needId != null && string.IsNullOrEmpty(model.StudentId))
            {
                return Request.CreateResponse(new { code = 1, email = needId.Email });
            }
            var id = User.GetCloudentsUserId();
            var command = new UpdateUserUniversityCommand(model.UniversityId, id, model.StudentId);
            try
            {
                m_ZboxWriteService.UpdateUserUniversity(command);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateBadRequestResponse(ex.Message);
            }


            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimConst.UserIdClaim, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));
            identity.AddClaim(new Claim(ClaimConst.UniversityIdClaim,
                    command.UniversityId.ToString(CultureInfo.InvariantCulture)));

            identity.AddClaim(new Claim(ClaimConst.UniversityDataClaim,
                    command.UniversityDataId.HasValue ?
                    command.UniversityDataId.Value.ToString(CultureInfo.InvariantCulture)
                    : command.UniversityId.ToString(CultureInfo.InvariantCulture)));

            var claims = identity.Claims.ToList();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));
            var token = AppServiceLoginHandler.CreateToken(claims,
         Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
        ConfigurationManager.AppSettings["ValidAudience"],
         ConfigurationManager.AppSettings["ValidIssuer"],
        TimeSpan.FromDays(30));

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                User = new
                {
                    UserId = User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)
                },
                AuthenticationToken = token.RawData
            });



        }


        [HttpPost]
        [Route("api/account/resetPassword")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> ResetPassword(ResetPasswordRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var query = new GetUserByEmailQuery(model.Email);
            var tUser = m_UserManager.FindByEmailAsync(model.Email);
            var tResult = m_ZboxReadService.GetUserDetailsByEmail(query);

            await Task.WhenAll(tUser, tResult);
            if (tUser.Result == null && tResult.Result != null)
            {
                return Request.CreateBadRequestResponse("You have registered to Spitball through Facebook -- go to the homepage and click on the Facebook button to register");
            }
            if (tUser.Result == null)
            {
                return Request.CreateBadRequestResponse("email doesn't exists");
            }
            if (tResult.Result == null)
            {
                return Request.CreateBadRequestResponse();
            }
            var user = tUser.Result;
            var identitylinkData = await m_UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var code = RandomString(4);
            await m_QueueProvider.InsertMessageToMailNewAsync(new ForgotPasswordData2(code, identitylinkData, tResult.Result.Name.Split(' ')[0], model.Email, tResult.Result.Culture));

            return Request.CreateResponse(new { code, resetToken = identitylinkData, user.Id });
        }

        //TODO: something is not right in here
        [HttpPost]
        [Route("api/account/passwordUpdate")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> PasswordUpdate(PasswordUpdateRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var result = await m_UserManager.ResetPasswordAsync(model.UserId, model.ResetToken, model.NewPassword);

            if (!result.Succeeded)
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    new HttpError("something went wrong, try again later"));
            var query = new GetUserByMembershipQuery(Guid.Parse(model.UserId));
            var tSystemUser = m_ZboxReadService.GetUserDetailsByMembershipId(query);

            var tUser = m_UserManager.FindByIdAsync(model.UserId);
            await Task.WhenAll(tSystemUser, tUser);

            var identity = await tUser.Result.GenerateUserIdentityAsync(m_UserManager, tSystemUser.Result.Id,
                tSystemUser.Result.UniversityId, tSystemUser.Result.UniversityData);

            var claims = identity.Claims.ToList();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)));
            var token = AppServiceLoginHandler.CreateToken(claims,
         Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
        ConfigurationManager.AppSettings["ValidAudience"],
         ConfigurationManager.AppSettings["ValidIssuer"],
        TimeSpan.FromDays(30));

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                User = new
                {
                    UserId = User.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture)
                },
                AuthenticationToken = token.RawData
            });


        }

        [HttpPost]
        [Route("api/account/logOff")]
        public HttpResponseMessage LogOff()
        {
            return Request.CreateResponse();
        }


        

        private static string RandomString(int size)
        {
            var random = new Random();
            const string input = "0123456789";
            var chars = Enumerable.Range(0, size)
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
            //return "12345";
        }

    }
}
