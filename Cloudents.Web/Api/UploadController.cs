using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Storage;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;

        public UploadController(IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _blobProvider = blobProvider;
        }

        // GET
        [HttpPost("ask")]
        public async Task<IActionResult> UploadFileAsync(UploadFileRequest model,
            [FromServices] UserManager<User> userManager,
            CancellationToken token)
        {
            var userId = userManager.GetUserId(User);

            var tasks = model.File.Select(s =>
            {
                var fileName = $"{userId}.{Guid.NewGuid()}.{s.FileName}";
                return _blobProvider
                    .UploadStreamAsync(fileName, s.OpenReadStream(), s.ContentType, false, 60 * 24, token).ContinueWith(_ => fileName);
            });
            var result = await Task.WhenAll(tasks).ConfigureAwait(false);
            return Ok(new
            {
                files = result
            });
        }
    }
}