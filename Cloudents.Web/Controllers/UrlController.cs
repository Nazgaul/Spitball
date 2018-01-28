using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class UrlController : Controller
    {
        public async Task<IActionResult> Index(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseUri = response.RequestMessage.RequestUri.ToString();
                return Content("hi ram" + responseUri);
            }
        }
    }
}