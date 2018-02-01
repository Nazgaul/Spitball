using System;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class UrlController : Controller
    {
        private readonly IRestClient _client;

        public UrlController(IRestClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index(string url)
        {
            var p = await _client.UrlRedirectAsync(new Uri(url)).ConfigureAwait(false);
            return Redirect(p.AbsoluteUri);
        }
    }
}