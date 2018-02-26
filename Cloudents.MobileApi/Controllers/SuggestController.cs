using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.MobileApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SuggestController : Controller
    {
        private readonly ISuggestions _suggestions;

        public SuggestController(ISuggestions suggestions)
        {
            _suggestions = suggestions;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string sentence,CancellationToken token)
        {
            var result = await _suggestions.SuggestAsync(sentence, token).ConfigureAwait(false);
            return Json(new
            {
                result
            });
        }
    }
}