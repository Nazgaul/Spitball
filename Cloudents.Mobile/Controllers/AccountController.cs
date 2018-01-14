using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Account controller
    /// </summary>
    [MobileAppController, Authorize ,Obsolete]
    public class AccountController : ApiController
    {
        //private readonly IZboxWriteService _zboxWriteService;
        private readonly IZboxReadService _zboxReadService;
        //private readonly IProfilePictureProvider _profilePicture;

        public AccountController(IZboxReadService zboxReadService)
        {
            //_zboxWriteService = zboxWriteService;
            _zboxReadService = zboxReadService;
            //_profilePicture = profilePicture;
        }

        // GET api/Account
        public async Task<HttpResponseMessage> Get(CancellationToken token)
        {
            try
            {
                var result = await _zboxReadService.GetJaredUserDataAsync(new QueryBaseUserId(User.GetUserId()), token)
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
            //if (!ModelState.IsValid)
            //{
            //    return Request.CreateBadRequestResponse();
            //}

            //var command = new UpdateUserProfileCommand(User.GetUserId(),
            //    model.FirstName,
            //    model.LastName);
            //_zboxWriteService.UpdateUserProfile(command);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost, Route("api/account/image")]
        public async Task<string> ProfilePictureAsync()
        {
            var bytes = await Request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            using (var ms = new MemoryStream(bytes))
            {
                //var result = await _profilePicture.UploadProfilePicturesAsync(ms).ConfigureAwait(false);
                //var command = new UpdateUserProfileImageCommand(User.GetUserId(), result.Image.AbsoluteUri);
                //_zboxWriteService.UpdateUserImage(command);
                return null;
            }
        }

        [HttpPost]
        [Route("api/account/university")]
        public HttpResponseMessage UpdateUniversity(UpdateUniversityRequest model)
        {
            //if (model == null)
            //{
            //    return Request.CreateBadRequestResponse();
            //}
            //if (!ModelState.IsValid)
            //{
            //    return Request.CreateBadRequestResponse();
            //}

            //var id = User.GetUserId();
            //var command = new UpdateUserUniversityCommand(model.UniversityId, id, null);
            //try
            //{
            //    _zboxWriteService.UpdateUserUniversity(command);
            //}
            //catch (ArgumentException ex)
            //{
            //    return Request.CreateBadRequestResponse(ex.Message);
            //}

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
