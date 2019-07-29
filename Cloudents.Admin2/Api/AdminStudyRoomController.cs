﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Query;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var country = User.Claims.Where(w => w.Type == "Country").First().Value;
            var query = new AdminStudyRoomQuery(country);
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }
    }
}
