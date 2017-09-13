using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Core.Interfaces;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/AI")]
    public class AIController : Controller
    {
        private readonly IAI m_AI;

        public AIController(IAI ai)
        {
            m_AI = ai;
        }

        [HttpGet]
        public async Task<IActionResult> AiAsync(string sentence)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));
            var result = await m_AI.InterpetStringAsync(sentence);
            return Json(result);
        }
    }
}