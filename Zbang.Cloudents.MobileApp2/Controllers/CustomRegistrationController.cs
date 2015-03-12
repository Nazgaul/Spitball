using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Domain.Commands;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.Application)]
    public class CustomRegistrationController : ApiController
    {
        public ApiServices Services { get; set; }
        public ApplicationUserManager UserManager { get; set; }
        public IZboxWriteService ZboxWriteService { get; set; }
        public IServiceTokenHandler Handler { get; set; }

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
            var createStatus = await UserManager.CreateAsync(user, model.Password);

            if (createStatus.Succeeded)
            {


                CreateUserCommand command = new CreateMembershipUserCommand(Guid.Parse(user.Id),
                    model.NewEmail, null, model.FirstName, model.LastName,
                    !model.IsMale.HasValue || model.IsMale.Value,
                    false, model.Culture, null, null, true);
                var result = await ZboxWriteService.CreateUserAsync(command);

                var identity = await user.GenerateUserIdentityAsync(UserManager, result.User.Id, result.UniversityId,
                     result.UniversityData);

                var loginResult = new Models.CustomLoginProvider(Handler)
                        .CreateLoginResult(identity, Services.Settings.MasterKey);
                return Request.CreateResponse(HttpStatusCode.OK, loginResult);

            }
            Services.Log.Error(string.Join(" ", createStatus.Errors));
            return Request.CreateBadRequestResponse();

        }

    }
}
