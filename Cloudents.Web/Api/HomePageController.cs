﻿using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Query;
using Cloudents.Query.Query;

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
            return await queryBus.QueryAsync(query, token);
        }
    }
}