﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [AllowAnonymous]
    public class ShoppingController : Controller
    {
        // GET: Shopping
        public ActionResult Index()
        {
            return View();
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