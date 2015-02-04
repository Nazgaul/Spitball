using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.MobileApp.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Anonymous)]
    public class CustomRegistrationController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxWriteService ZboxWriteService { get; set; }

        public IServiceTokenHandler Handler { get; set; }

        public IMembershipService MembershipService { get; set; }

        // GET api/CustomRegistration
        public async Task<HttpResponseMessage> Post(Register registrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var userid = Guid.NewGuid().ToString();
            try
            {
                Guid userProviderKey;
                var createStatus = MembershipService.CreateUser(userid, registrationRequest.Password, registrationRequest.NewEmail,
                    out userProviderKey);
                if (createStatus == MembershipCreateStatus.Success)
                {


                    CreateUserCommand command = new CreateMembershipUserCommand(userProviderKey,
                        registrationRequest.NewEmail, null, registrationRequest.FirstName, registrationRequest.LastName,
                        !registrationRequest.IsMale.HasValue || registrationRequest.IsMale.Value,
                        false, CultureInfo.CurrentCulture.Name, null, null, true);
                    var result = await ZboxWriteService.CreateUserAsync(command);


                    var claimsIdentity = new ClaimsIdentity();
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.User.ToString()));
                    var loginResult = new CustomLoginProvider(Handler).CreateLoginResult(claimsIdentity, Services.Settings.MasterKey);
                    return Request.CreateResponse(HttpStatusCode.OK, loginResult);
                }
                ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(createStatus));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong");
                Services.Log.Error("Register model:" + registrationRequest, ex);
            }
            return Request.CreateBadRequestResponse();

        }

    }
}
