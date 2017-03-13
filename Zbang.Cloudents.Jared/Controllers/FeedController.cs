using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    [Authorize]
    public class FeedController : ApiController
    {
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IZboxWriteService m_ZboxWriteService;

        public FeedController(IGuidIdGenerator guidGenerator, IZboxWriteService zboxWriteService)
        {
            m_GuidGenerator = guidGenerator;
            m_ZboxWriteService = zboxWriteService;
        }

        [HttpPost]
       
        [Route("api/course/{boxId:long}/feed")]
        public async Task<HttpResponseMessage> PostCommentAsync(long boxId, AddCommentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var questionId = m_GuidGenerator.GetId();
            var command = new AddCommentCommand(User.GetUserId(),
                boxId, model.Content, questionId, null, false);
            await m_ZboxWriteService.AddCommentAsync(command).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
