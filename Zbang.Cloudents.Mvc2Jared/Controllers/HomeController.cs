using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc2Jared.Models;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries.Jared;
using System.Threading;
using Zbang.Zbox.ReadServices;
using System.Linq;

namespace Zbang.Cloudents.Mvc2Jared.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : Controller
    {
        IZboxReadService m_readService;
        public HomeController(IZboxReadService readService)
        {
            m_readService = readService;
        }

        public ActionResult Page()
        {
            return View();
        }
     
        [HttpPost, ActionName("Items")]
        public async Task<JsonResult> ItemsAsync(JaredSearchQuery model, CancellationToken cancellationToken)
        {
           var retVal = await m_readService.GetItemsWithTagsAsync(model);
           return Json(retVal);
        }
    }
}