using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class AiController : ApiController
    {
        private readonly IEngineProcess _engineProcess;

        public AiController(IEngineProcess engineProcess)
        {
            _engineProcess = engineProcess;
        }


        // GET api/Ai
        public async Task<HttpResponseMessage> Get(string sentence)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));
            var result = await _engineProcess.ProcessRequestAsync(sentence).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
