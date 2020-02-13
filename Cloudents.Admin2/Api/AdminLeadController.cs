using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminLeadController : ControllerBase
    {
        
        private readonly IQueryBus _queryBus;

        public AdminLeadController( IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }
        [HttpGet]
        //[Authorize(Policy = Policy.IsraelUser)]
        public async Task<IEnumerable<LeadDto>> LeadAsync(CancellationToken token)
        {
            var query = new LeadsQuery( User.GetCountryClaim());
            return await _queryBus.QueryAsync(query, token);
        }
        
    }
}
