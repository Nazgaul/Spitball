using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Suggest")]
    public class SuggestController : Controller
    {
        public IActionResult Get(string sentence)
        {
            return Json(new[]
            {
                sentence,
                sentence + "1",
                sentence + "2"
            });
        }
    }
}