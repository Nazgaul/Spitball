using System;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Query.General;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class GoController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly IGoogleAnalytics _googleAnalytics;


        public GoController(IQueryBus queryBus, IGoogleAnalytics googleAnalytics)
        {
            _googleAnalytics = googleAnalytics;
            _queryBus = queryBus;
        }


        // GET: /<controller>/
        [Route("go/{identifier}")]
        public async Task<RedirectResult> Index(string identifier, [FromQuery]string site,
            [FromQuery]string eventCategory,
            [FromQuery]string eventAction,
            [FromQuery]string eventLabel,
            CancellationToken token)
        {

            var query = new ShortUrlQuery(identifier);
            var result = await _queryBus.QueryAsync(query, token);
            if (result == null)
            {
                return Redirect("/");
            }
            if (eventCategory != null && eventAction != null && eventLabel != null)
            {
                await _googleAnalytics.TrackEventAsync(eventCategory, eventAction, eventLabel);
            }
            if (!Uri.TryCreate(result.Destination, UriKind.RelativeOrAbsolute, out var uri))
            {
                return RedirectPermanent(result.Destination);
            }
            if (uri.IsAbsoluteUri)
            {
                return RedirectPermanent(result.Destination);
            }
            var destination = result.Destination;
            if (site?.Equals("frymo", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (destination.Contains("?"))
                {
                    destination += "&site=frymo";
                }
                else
                {
                    destination += "?site=frymo";
                }
            }
            return RedirectPermanent(destination);
        }
    }
}
