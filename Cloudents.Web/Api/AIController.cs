using System;
using System.Threading.Tasks;
using Cloudents.Core;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AIController : Controller
    {
        private readonly IAI m_AI;
        private readonly IDecision m_Decision;

        public AIController(IAI ai, IDecision decision)
        {
            m_AI = ai;
            m_Decision = decision;
        }

        [HttpGet]
        //[ResponseCache(VaryByQueryKeys = new[] { "sentence" }, Duration = 30 * 60)]
        public async Task<IActionResult> AiAsync(string sentence)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));
            var query = new AiQuery(sentence);
            var aiResult = await m_AI.InterpretStringAsync(query).ConfigureAwait(false);
            var result = m_Decision.MakeDecision(aiResult);
            return Json(new
            {
                result.result,
                result.data
            });
        }
    }
}