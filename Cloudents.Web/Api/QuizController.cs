using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Quiz")]
    public class QuizController : Controller
    {
        private readonly IReadRepositoryAsync<QuizDto, long> _repository;

        public QuizController(IReadRepositoryAsync<QuizDto, long> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Get(long id, CancellationToken token)
        {
            //TODO: need to add to queue extra view
            var result = await _repository.GetAsync(id, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}