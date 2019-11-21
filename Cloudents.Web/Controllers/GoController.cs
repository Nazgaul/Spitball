using System;
using Cloudents.Query;
using Cloudents.Query.Query;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class GoController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly IHostingEnvironment _hostingEnvironment;

        public GoController(IQueryBus queryBus, IHostingEnvironment configuration)
        {
            _queryBus = queryBus;
            _hostingEnvironment = configuration;
        }

        // GET: /<controller>/
        [Route("go/{identifier}")]
        public async Task<IActionResult> Index(string identifier, [FromQuery]string site, CancellationToken token)
        {

            var query = new ShortUrlQuery(identifier);
            var result = await _queryBus.QueryAsync(query, token);
            if (result == null)
            {
                return NotFound();
            }

            Uri.TryCreate(result.Destination, UriKind.RelativeOrAbsolute, out var uri);
            if (uri.IsAbsoluteUri)
            {
                return RedirectPermanent(result.Destination);
            }
            //if (site?.Equals("frymo", StringComparison.OrdinalIgnoreCase) == true)
            //{

            //    if (_hostingEnvironment.IsProduction())
            //    {
            //        //const string absoluteUri = "https://www.sp"
            //    }
            //}

            //if ()

            return RedirectPermanent(result.Destination);
        }
    }
}
