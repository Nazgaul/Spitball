using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class BlogController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return Redirect("https://medium.com/@spitballstudy");
        }
    }
}