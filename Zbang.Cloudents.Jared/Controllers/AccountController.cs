using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController, Authorize]
    public class AccountController : ApiController
    {
        private readonly IZboxWriteService m_ZboxWriteService;

        public AccountController(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        // GET api/Account
        public string Get()
        {
            return "Hello from custom controller!";
        }


        [HttpPost]
        [Route("api/Account/profile")]
        public HttpResponseMessage ChangeProfile(ProfileRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var command = new UpdateUserProfileCommand(User.GetUserId(),
                model.FirstName,
                model.LastName);
            m_ZboxWriteService.UpdateUserProfile(command);
            return Request.CreateResponse(HttpStatusCode.OK);

        }
    }
}
