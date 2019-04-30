using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class WalletController : Controller
    {
        // GET
        public IActionResult Payment(string query)
        {
            return
            View();
        }
    }
}