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
using System;
using System.IO;
using System.Linq;
using System.Threading;
using Zbang.Zbox.Infrastructure.Profile;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController, Authorize]
    public class AccountController : ApiController
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IZboxReadService m_ZboxReadService;
        private readonly IProfilePictureProvider m_ProfilePicture;

        public AccountController(IZboxWriteService zboxWriteService, IZboxReadService zboxReadService, IProfilePictureProvider profilePicture)
        {
            m_ZboxWriteService = zboxWriteService;
            m_ZboxReadService = zboxReadService;
            m_ProfilePicture = profilePicture;
        }

        // GET api/Account
        public async Task<HttpResponseMessage> Get(CancellationToken token)
        {
            try
            {
                var result = await m_ZboxReadService.GetJaredUserDataAsync(new QueryBaseUserId(User.GetUserId()), token)
                    .ConfigureAwait(false);
                return Request.CreateResponse(new
                {
                    university = new
                    {
                        Id = result.Item1.UniversityId,
                        Name = result.Item1.UniversityName
                    },
                    courses = result.Item2.Select(s => new
                    {
                        s.Id,
                        s.Name,
                        code = s.CourseCode
                    })
                });
            }
            catch (InvalidOperationException)
            {
                return Request.CreateNotFoundResponse();
            }
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

        [HttpPost, Route("api/account/image")]
        public async Task<string> ProfilePictureAsync()
        {
            var bytes = await Request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            using (var ms = new MemoryStream(bytes))
            {
                var result = await m_ProfilePicture.UploadProfilePicturesAsync(ms).ConfigureAwait(false);
                var command = new UpdateUserProfileImageCommand(User.GetUserId(), result.Image.AbsoluteUri);
                m_ZboxWriteService.UpdateUserImage(command);
                return result.Image.AbsoluteUri;
            }
        }

        [HttpPost]
        [Route("api/account/university")]

        public HttpResponseMessage UpdateUniversity(UpdateUniversityRequest model)
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
