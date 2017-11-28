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
        private readonly IAI _ai;
        private readonly IDecision _mDecision;

        public AiController(IAI ai, IDecision mDecision)
        {
            _ai = ai;
            _mDecision = mDecision;
        }

        // GET api/Ai
        public async Task<HttpResponseMessage> Get(string sentence)
        {
            if (sentence == null) throw new ArgumentNullException(nameof(sentence));
            var aiResult = await _ai.InterpretStringAsync(sentence).ConfigureAwait(false);
            var result = _mDecision.MakeDecision(aiResult);
            return Request.CreateResponse(new
            {
                result.result,
                result.data
            });
        }
    }
}
