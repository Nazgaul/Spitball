﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class QuizController : Controller
    {
        [Route("quiz/{universityName}/{boxId:long}/{boxName}/{id:long}/{name}")]
        public IActionResult Index()
        {
            return this.RedirectToOldSite();
            
        }
    }
}