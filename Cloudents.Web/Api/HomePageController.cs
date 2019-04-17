using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Query;
using Cloudents.Query.Query;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public HomePageController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet]
        [ResponseCache(Duration = TimeConst.Day, Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<StatsDto>> GetAsync(CancellationToken token)
        {
            var query = new HomePageQuery();
            return await _queryBus.QueryAsync(query, token);
        }
    }
}
