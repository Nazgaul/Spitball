using System.Threading;
using System.Threading.Tasks;
using Cloudents.Api.Extensions;
using Cloudents.Api.Filters;
using Cloudents.Api.Models;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Get a sentence the user enter and interpret it
    /// </summary>
    [Route("api/[controller]")]
    public class AiController : Controller
    {
        private readonly IEngineProcess _engineProcess;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engineProcess"></param>
        public AiController(IEngineProcess engineProcess)
        {
            _engineProcess = engineProcess;
        }

        /// <summary>
        /// interpret user sentence
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateModel]
        [ProducesResponseType(typeof(VerticalEngineDto), 200)]
        public async Task<IActionResult> GetAsync(
           AiRequest model,
           CancellationToken token)
        {
            var t = Request.GetCapabilities();
            var result = await _engineProcess.ProcessRequestAsync(model.Sentence, token).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
