using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.WebApi.Models;

namespace Zbang.Zbox.WebApi.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [Dependency]
        IZboxApiReadService m_ZboxReadService { get; set; }


        public ActionResult Index()
        {
            // GlobalConfiguration.Configuration.Services.Replace(typeof(IDocumentationProvider), new Documentation());
            var explorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();
            return View(new ApiModel(explorer));
        }

    }
}
