﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Core.Spitball.Controllers
{
    public class ResultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}