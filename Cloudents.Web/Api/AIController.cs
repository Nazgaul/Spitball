﻿using System;
using System.Threading.Tasks;
using Cloudents.Core;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Core.Interfaces;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
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
            //var navigation = BuildFlow(result.result);
            return Json(new
            {
                result.result,
                result.data
            });
        }

        //private Tree<string> BuildFlow(AIResult aiResult)
        //{
        //    var t = new Tree<string>("document");
        //    var f = new Tree<string>("flashcard");
        //    var p = new Tree<string>("post");
        //    var tu = new Tree<string>("tutor");

        //    p.Add(tu);
        //    f.Add(p);
        //    t.Add(f);

        //    return t;

        //}
    }
}