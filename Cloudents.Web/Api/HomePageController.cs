using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        [HttpGet]
        [ResponseCache(Duration = TimeConst.Day,Location = ResponseCacheLocation.Any)]
        public async Task<ActionResult<StatsDto>> GetAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            var query = new HomePageQuery();
            return await queryBus.QueryAsync<StatsDto>(query, token);
        }
    }
}
