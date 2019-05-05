using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            return Redirect("https://india.spitball.co/tutoring");
        }

        [Route("tutoring/{id:guid}")]
        public IActionResult IndexWithId(Guid id)
        {
            return Redirect($"https://india.spitball.co/tutoring/{id}");
        }
    }
}
