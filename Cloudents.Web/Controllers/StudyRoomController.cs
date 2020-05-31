using System;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StudyRoomController : Controller
    {
        [Route("StudyRoom/{id:guid}")]
        public IActionResult Index(Guid id, [FromQuery] string? dialog)
        {
            ViewBag.isRtl = false;
            if (dialog == "payment")
            {
                // this hotfix that happens on client side
                return RedirectToAction("Index");
            }
            return View("Index");
        }
    }
}