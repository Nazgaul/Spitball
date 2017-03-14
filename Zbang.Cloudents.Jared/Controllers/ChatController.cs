using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class ChatController : ApiController
    {
        // GET api/Chat
        public string Get()
        {
            return "Hello from custom controller!";
        }

        [HttpGet]
        [Route("api/chat/online")]
        public async Task<HttpResponseMessage> OnlineUsersAsync(long boxId)
        {
            
        }
    }
}
