using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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