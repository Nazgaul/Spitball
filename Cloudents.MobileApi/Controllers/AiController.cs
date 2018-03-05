using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.MobileApi.Controllers
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
        /// <param name="sentence">The sentence</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(VerticalEngineDto), 200)]
        public async Task<IActionResult> GetAsync(
            string sentence, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return BadRequest();
            }
            var result = await _engineProcess.ProcessRequestAsync(sentence, token).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
