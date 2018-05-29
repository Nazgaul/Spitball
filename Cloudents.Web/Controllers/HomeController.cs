﻿using System;
using System.Collections.Generic;
using System.Net;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly List<IPAddress> _officeIps = new List<IPAddress>();

        public HomeController( IConfiguration configuration)
        {
            var ipsStr = configuration["Ips"];

            if (ipsStr == null) return;
            foreach (var ipStr in ipsStr.Split(','))
            {
                if (IPAddress.TryParse(ipStr, out var ip))
                {
                    _officeIps.Add(ip);
                }
            }
        }

        //[ResponseCache()]
        // we can't use that for now.
        // GET
        public IActionResult Index(LocationQuery location, [FromServices]IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                return View();
            }

            if (env.IsStaging())
            {
                return View();
            }
            var requestIp = HttpContext.Connection.GetIpAddress();
            if (_officeIps.Contains(requestIp))
            {
                return View();
            }
            if (!string.Equals(location?.Address?.CountryCode, "US", StringComparison.OrdinalIgnoreCase))
            {
                return this.RedirectToOldSite();
            }
            return View();
        }
    }
}