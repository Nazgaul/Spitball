using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Cloudents.Jared.DataObjects;
using System;
using System.Linq;
using System.Threading;

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
        public async Task<HttpResponseMessage> Get(CancellationToken token)
        {
            var result = await m_ZboxReadService.GetJaredUserDataAsync(new QueryBaseUserId(User.GetUserId()), token).ConfigureAwait(false);
            return Request.CreateResponse(new
            {
                university = new
                {
                   Id =  result.Item1.UniversityId,
                   Name = result.Item1.UniversityName
                },
                courses = result.Item2.Select(s => new
                {
                    s.Id,
                    s.Name
                })
            });
            //return Request.CreateResponse(new
            //{
            //    result.UniversityId,
            //    result.UniversityName

            //});
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
        [HttpPost]
        [Route("api/account/university")]

        public HttpResponseMessage UpdateUniversityAsync(UpdateUniversityRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var id = User.GetUserId();
            var command = new UpdateUserUniversityCommand(model.UniversityId, id, null);
            try
            {
                m_ZboxWriteService.UpdateUserUniversity(command);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateBadRequestResponse(ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
