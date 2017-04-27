using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Net.Http;
using System.Threading.Tasks;
using Zbang.Cloudents.Jared.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Url;
using System.Net;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class CourseController : ApiController
    {
        private readonly IZboxWriteService m_ZboxWriteService;
        public CourseController(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }
        
        // GET api/Course
        public string Get()
        {
            return "Hello from custom controller!";
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
            await m_ZboxWriteService.SubscribeToSharedBoxAsync(command);
            return Request.CreateResponse();
        }
        [Route("api/course/create")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateAcademicBoxAsync(CreateAcademicCourseRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            

            var commandGeneral = new GetGeneralDepartmentCommand() { UserId = User.GetUserId(), UniversityId = model.UniversityId };
            var res = m_ZboxWriteService.GetGeneralDepartmentForUni(commandGeneral);
           
            try
            {
                var userId = User.GetUserId();

                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, model.Professor, res.departmentId.Value, model.UniversityId);
                var result = await m_ZboxWriteService.CreateBoxAsync(command);
                return Request.CreateResponse(new
                {
                    result.Url,
                    result.Id,
                    shortUrl = UrlConst.BuildShortBoxUrl(new Base62(result.Id).ToString())
                });
            }
            catch (BoxNameAlreadyExistsException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "box already exists");

            }
        }
    }
}
