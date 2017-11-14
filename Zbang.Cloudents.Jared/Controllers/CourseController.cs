using System;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Net.Http;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Exceptions;
using System.Net;
using System.Threading;
using Cloudents.Core.Interfaces;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class CourseController : ApiController
    {
        private readonly IZboxWriteService _zboxWriteService;
        private readonly ICourseSearch _courseProvider;
        public CourseController(IZboxWriteService zboxWriteService, ICourseSearch courseProvider)
        {
            _zboxWriteService = zboxWriteService;
            _courseProvider = courseProvider;
        }

        [Route("api/course/search")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get(string term, long universityId, CancellationToken token)
        {
            if (universityId == default)
            {
                throw new ArgumentException(nameof(universityId));
            }
            var result = await _courseProvider.SearchAsync(term, universityId, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }

        [Route("api/course/follow")]
        [HttpPost]
        public async Task<HttpResponseMessage> FollowAsync(FollowRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new SubscribeToSharedBoxCommand(User.GetUserId(), model.BoxId);
            await _zboxWriteService.SubscribeToSharedBoxAsync(command).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete, Route("api/course/follow")]
        public async Task<HttpResponseMessage> UnFollowAsync(long courseId)
        {
            var command = new UnFollowBoxCommand(courseId, User.GetUserId(), false);
            await _zboxWriteService.UnFollowBoxAsync(command).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/course/create")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateAcademicBoxAsync(CreateAcademicCourseRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var userId = User.GetUserId();
            var commandGeneral = new GetGeneralDepartmentCommand(userId);
            var res = _zboxWriteService.GetGeneralDepartmentForUniversity(commandGeneral);

            try
            {

                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, null, res.DepartmentId);
                var result = await _zboxWriteService.CreateBoxAsync(command).ConfigureAwait(false);
                return Request.CreateResponse(result.Id);
            }
            catch (BoxNameAlreadyExistsException ex)
            {
                return Request.CreateResponse(ex.BoxId);
            }
        }
    }
}
