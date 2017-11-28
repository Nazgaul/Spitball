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
        private readonly IEngineProcess _engineProcess;

        public AiController(IEngineProcess engineProcess)
        {
            _engineProcess = engineProcess;
        }


        [HttpGet]
        [ValidateModel]
        //[ResponseCache(VaryByQueryKeys = new[] { "sentence" }, Duration = 30 * 60)]
        public async Task<IActionResult> AiAsync(
            AiRequest model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var result = await _engineProcess.ProcessRequestAsync(model.Sentence).ConfigureAwait(false);
            return Json(result);
        }
    }
}