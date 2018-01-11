using System.Web.Http;
using Cloudents.Mobile.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Mobile.Controllers
{
    [Route(".auth/refresh")]
    public class RefreshController : ApiController
    {
        // GET api/Refresh
        public string Get()
        {
            return "Hello from custom controller!";
        }

        public IHttpActionResult Post([FromBody] JObject assertion)
        {
            return Ok(new LoginResult
            {
                AuthenticationToken = string.Empty,
                User = new LoginResultUser
                { UserId = string.Empty }
            });
        }
    }
}
