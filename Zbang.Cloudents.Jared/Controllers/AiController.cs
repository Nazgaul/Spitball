using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <summary>
    /// Get a sentence the user enter and interpret it
    /// </summary>
    [MobileAppController]
    public class AiController : ApiController
    {
        private readonly IEngineProcess _engineProcess;

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
        public async Task<HttpResponseMessage> Get(string sentence, CancellationToken token)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));
            var result = await _engineProcess.ProcessRequestAsync(sentence, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
