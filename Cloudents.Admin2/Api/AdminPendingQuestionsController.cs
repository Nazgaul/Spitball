using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using Microsoft.AspNetCore.Mvc;


namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    public class AdminPendingQuestionsController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;


        public AdminPendingQuestionsController(ICommandBus commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        /// <summary>
        /// Get a list of question with pending state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<QuestionDto>> Get(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var t = await _queryBus.QueryAsync<IEnumerable<QuestionDto>>(query, token);
            return t.Take(100);
        }

    }
}
