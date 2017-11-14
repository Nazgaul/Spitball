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
    [Route("api/Document")]
    public class DocumentController : Controller
    {
        private readonly IReadRepositoryAsync<DocumentDto, long> _repository;

        public DocumentController(IReadRepositoryAsync<DocumentDto, long> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Get(long id, bool? firstTime, CancellationToken token)
        {
            var model = await _repository.GetAsync(id, token).ConfigureAwait(false);
            if (firstTime.GetValueOrDefault())
            {
                //TODO: need to bring content of blob
            }
            if (model == null)
            {
                return NotFound();
            }
            return Json(model);
        }
    }
}