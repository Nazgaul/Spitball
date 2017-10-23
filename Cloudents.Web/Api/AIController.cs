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
    public class AIController : Controller
    {
        private readonly IAI m_Ai;
        private readonly IDecision m_Decision;

        public AIController(IAI ai, IDecision decision)
        {
            m_Ai = ai;
            m_Decision = decision;
        }

        [HttpGet]
        [ValidateModel]
        //[ResponseCache(VaryByQueryKeys = new[] { "sentence" }, Duration = 30 * 60)]
        public async Task<IActionResult> AiAsync(
            AiRequest model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var aiResult = await m_Ai.InterpretStringAsync(model.Sentence).ConfigureAwait(false);
            var result = m_Decision.MakeDecision(aiResult);
            return Json(new
            {
                result.result,
                result.data
            });
        }
    }
}