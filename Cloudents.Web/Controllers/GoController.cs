using Cloudents.Query;
using Cloudents.Query.Query;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class GoController : Controller
    {
        private readonly IQueryBus _queryBus;

        public GoController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        // GET: /<controller>/
        [Route("go/{identifier}")]
        public async Task<IActionResult> Index(string identifier, CancellationToken token)
        {
            var query = new ShortUrlQuery(identifier);
            var result = await _queryBus.QueryAsync(query, token);
            if (result == null)
            {
                return NotFound();
            }

            return RedirectPermanent(result.Destination);
        }
    }
}
