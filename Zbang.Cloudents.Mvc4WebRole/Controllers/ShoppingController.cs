using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [AllowAnonymous]
    public class ShoppingController : BaseController
    {
        // GET: Shopping
        public async Task<ActionResult> Index()
        {
            var products = await ZboxReadService.GetProducts();
            return View(products);
        }

        public ActionResult Product()
        {
            return View();
        }

        public ActionResult CheckOut()
        {
            return View();
        }
    }
}