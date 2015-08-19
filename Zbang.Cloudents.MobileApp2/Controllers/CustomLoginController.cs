﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Dto.UserDtos;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Application)]
    public class CustomLoginController : ApiController
    {
        public ApiServices Services { get; set; }
        public IServiceTokenHandler Handler { get; set; }


        public IZboxReadService ZboxReadService { get; set; }
        public IZboxWriteService ZboxWriteService { get; set; }

        public IFacebookService FacebookService { get; set; }

        public ApplicationUserManager UserManager { get; set; }

        // GET api/CustomLogin
        public async Task<HttpResponseMessage> Post(LogInRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                Services.Log.Error("model is not valid " + loginRequest);
                return Request.CreateBadRequestResponse();
            }
            try
            {
                var query = new GetUserByEmailQuery(loginRequest.Email);

                var user = await UserManager.FindByEmailAsync(loginRequest.Email);

                //await Task.WhenAll(tSystemData, tUserIdentity);


                var tSystemData = ZboxReadService.GetUserDetailsByEmail(query);
                var tLogIn = UserManager.CheckPasswordAsync(user, loginRequest.Password);
                await Task.WhenAll(tSystemData, tLogIn);
                var systemUser = tSystemData.Result;
                if (systemUser == null)
                {
                    Services.Log.Error("system user is not valid " + loginRequest);
                    return Request.CreateBadRequestResponse();
                }
                if (tLogIn.Result)
                {
                    var identity = await user.GenerateUserIdentityAsync(UserManager, systemUser.Id, systemUser.UniversityId,
                         systemUser.UniversityData);
                    var loginResult = new Models.CustomLoginProvider(Handler)
                        .CreateLoginResult(identity, Services.Settings.MasterKey);
                    return Request.CreateResponse(HttpStatusCode.OK, loginResult);
                }

                Services.Log.Error("tLogIn result is false " + loginRequest);

            }
            catch (Exception ex)
            {

                Services.Log.Error(string.Format("LogOn model : {0} ", loginRequest), ex);
                //ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
            }
            return Request.CreateBadRequestResponse();

        }


        [HttpPost]
        [Route("api/facebookLogin")]
        public async Task<HttpResponseMessage> FacebookLogin(FacebookLoginRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            var facebookUserData = await FacebookService.FacebookLogIn(model.AuthToken);
            if (facebookUserData == null)
            {
                return Request.CreateBadRequestResponse("auth token is invalid");
            }
            try
            {
                var query = new GetUserByFacebookQuery(facebookUserData.Id);
                var user = await ZboxReadService.GetUserDetailsByFacebookId(query);
                if (user == null)
                {
                    var command = new CreateFacebookUserCommand(facebookUserData.Id, facebookUserData.Email,
                        facebookUserData.Image, facebookUserData.LargeImage, null,
                        facebookUserData.First_name,
                        facebookUserData.Middle_name,
                        facebookUserData.Last_name,
                        facebookUserData.GetGender(),
                        facebookUserData.Locale, null, null, true);
                    var commandResult = await ZboxWriteService.CreateUserAsync(command);
                    user = new LogInUserDto
                    {
                        Id = commandResult.User.Id,
                        Culture = commandResult.User.Culture,
                        Image = facebookUserData.Image,
                        Name = facebookUserData.Name,
                        UniversityId = commandResult.UniversityId,
                        UniversityData = commandResult.UniversityData,
                        Score = commandResult.User.Reputation
                    };
                }

                var identity = ApplicationUser.GenerateUserIdentity(user.Id, user.UniversityId, user.UniversityData);
                var loginResult = new Models.CustomLoginProvider(Handler)
                    .CreateLoginResult(identity, Services.Settings.MasterKey);
                return Request.CreateResponse(HttpStatusCode.OK, loginResult);
            }
            catch (Exception ex)
            {
                Services.Log.Error(string.Format("Error in facebook login - facebook data - {0} ex {1}", facebookUserData, ex));
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}
