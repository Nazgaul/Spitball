using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController, Authorize]
    public class AccountController : ApiController
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IZboxReadService m_ZboxReadService;

        public AccountController(IZboxWriteService zboxWriteService, IZboxReadService zboxReadService)
        {
            m_ZboxWriteService = zboxWriteService;
            m_ZboxReadService = zboxReadService;
        }

        // GET api/Account
        public async Task<HttpResponseMessage> Get()
        {
           var result = await m_ZboxReadService.GetUserDataAsync(new QueryBaseUserId(User.GetUserId())).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                result.UniversityId,
                result.UniversityName
                
            });
            //return "Hello from custom controller!";
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
