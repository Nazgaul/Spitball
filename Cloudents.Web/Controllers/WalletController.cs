using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class WalletController : Controller
    {
        // GET
        [Route("[controller]", Name = "WalletRoute")]
        public IActionResult Index()
        {
            return View();
        }
    }
}