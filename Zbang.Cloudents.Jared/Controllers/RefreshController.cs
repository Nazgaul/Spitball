using System.Web.Http;
using Newtonsoft.Json.Linq;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
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
