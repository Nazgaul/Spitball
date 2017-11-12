using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cloudents.Core;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AiController : Controller
    {
        private readonly IAI _ai;
        private readonly IDecision _mDecision;

        public AiController(IAI ai, IDecision decision)
        {
            _ai = ai;
            _mDecision = decision;
        }

        [HttpGet]
        [ValidateModel]
        //[ResponseCache(VaryByQueryKeys = new[] { "sentence" }, Duration = 30 * 60)]
        public async Task<IActionResult> AiAsync(
            AiRequest model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var aiResult = await _ai.InterpretStringAsync(model.Sentence).ConfigureAwait(false);
            var result = _mDecision.MakeDecision(aiResult);
            return Json(new
            {
                result.result,
                result.data
            });
        }
    }
}