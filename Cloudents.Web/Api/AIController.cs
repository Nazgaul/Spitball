using System;
using System.Threading.Tasks;
using Cloudents.Core;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Core.Interfaces;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/AI")]
    public class AIController : Controller
    {
        private readonly IAI m_AI;
        private readonly IDesicions m_Decision;

        public AIController(IAI ai, IDesicions decision)
        {
            m_AI = ai;
            m_Decision = decision;
        }

        [HttpGet]
        public async Task<IActionResult> AiAsync(string sentence)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));
            var aiResult = await m_AI.InterpetStringAsync(sentence).ConfigureAwait(false);
            var result = m_Decision.MakeDesicision(aiResult);

            return Json(result);
        }

        //private void BuildFlow(AIResult aiResult)
        //{
            
        //}
    }
}