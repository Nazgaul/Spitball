﻿using System;
using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TabsController : Controller
    {
        // GET: /<controller>/
        [Route("tutor")]
        [Route("book")]
        [Route("job")]
        [Route("note")]
        [Route("flashcard")]
        public IActionResult Index(
            [ModelBinder(typeof(CountryModelBinder))] string country)
        {
            if (string.Equals(country, "il", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.country = country ?? "us";
            return View();
        }
    }
}
