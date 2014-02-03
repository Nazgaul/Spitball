using System;
using System.Linq;
using System.Web.Mvc;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.MvcWebRole.Helpers;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.MvcWebRole.Models;

namespace Zbang.Zbox.MvcWebRole.Controllers
{
    [Authorize]
    [HandleError]
    public class HomeController: Controller
    {
        public HomeController()
        {
                                              
        }
        //Methods
        [CompressFilter]
        public ActionResult Index(string boxid)
        {
            ViewData["boxid"] = boxid;
            return View();
        }

       




    }
}
