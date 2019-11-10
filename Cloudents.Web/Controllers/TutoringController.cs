using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TutoringController : Controller
    {
        // GET: /<controller>/
        [Route("tutoring")]
        public IActionResult Index()
        {
            return Redirect("/");
        }

        [Route("tutoring/{id:guid}")]
        public IActionResult IndexWithId(Guid id)
        {
            return Redirect("/");
        }
    }
}
