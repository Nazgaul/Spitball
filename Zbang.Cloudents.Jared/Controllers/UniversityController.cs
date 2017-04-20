using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class UniversityController : ApiController
    {
        // GET api/University
        public string Get()
        {
            return "Hello from custom controller!";
        }
    }
}
