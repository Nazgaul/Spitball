using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Net.Http;
using System.Threading.Tasks;
using Zbang.Cloudents.Jared.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Exceptions;
using System.Net;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController,Authorize]
    public class CourseController : ApiController
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        public CourseController(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
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
            await m_ZboxWriteService.SubscribeToSharedBoxAsync(command).ConfigureAwait(false);
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
            

            var commandGeneral = new GetGeneralDepartmentCommand(User.GetUserId(), model.UniversityId);
            var res = m_ZboxWriteService.GetGeneralDepartmentForUniversity(commandGeneral);
           
            try
            {
                var userId = User.GetUserId();

                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, model.Professor, res.DepartmentId, model.UniversityId);
                var result = await m_ZboxWriteService.CreateBoxAsync(command).ConfigureAwait(false);
                return Request.CreateResponse(result.Id);
            }
            catch (BoxNameAlreadyExistsException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "box already exists");

            }
        }
    }
}
