using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StudyRoomController : Controller
    {
        [Route("studyroom/{id:guid}")]
        public IActionResult Index(Guid id)
        {
            ViewBag.isRtl = false;
            return View("Index");
        }
    }
}