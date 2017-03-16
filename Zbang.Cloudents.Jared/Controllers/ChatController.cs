using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class ChatController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;

        public ChatController(IZboxCacheReadService zboxReadService)
        {
            m_ZboxReadService = zboxReadService;
        }

        // GET api/Chat
        public string Get()
        {
            return "Hello from custom controller!";
        }

        [HttpGet]
        [Route("api/chat/online")]
        public async Task<HttpResponseMessage> OnlineUsersAsync(long boxId)
        {
           var retVal = await m_ZboxReadService.OnlineUsersByClassAsync(new GetBoxIdQuery(boxId)).ConfigureAwait(false);
            return Request.CreateResponse(retVal);
        }
    }
}
