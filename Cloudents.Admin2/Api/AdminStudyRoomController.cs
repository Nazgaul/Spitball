using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Query.Admin;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminStudyRoomController: ControllerBase
    {
        private readonly IQueryBus _queryBus;

        public AdminStudyRoomController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [HttpGet]
        public async Task<IEnumerable<StudyRoomDto>> StudyRoomAsync(CancellationToken token)
        {
            var query = new AdminStudyRoomQuery();
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }
    }
}
