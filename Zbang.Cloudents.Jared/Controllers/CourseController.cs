using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using System.Net.Http;
using System.Threading.Tasks;
using Zbang.Cloudents.Jared.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;

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
    }
}
