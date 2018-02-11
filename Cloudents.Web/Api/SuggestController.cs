using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Suggest")]
    public class SuggestController : Controller
    {
        private readonly ISuggestions _suggestions;

        public SuggestController(ISuggestions suggestions)
        {
            _suggestions = suggestions;
        }

        public async Task<IActionResult> Get(string sentence,CancellationToken token)
        {
            var result = await _suggestions.SuggestAsync(sentence, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}