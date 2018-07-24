using System;
using System.Drawing;
using System.IO;
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
        private readonly string[] _supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };


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
                if (!s.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("not an image");
                }

                var extension = Path.GetExtension(s.FileName);

                if (!_supportedImages.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("not an image");
                }

                using (var sr = s.OpenReadStream())
                {
                    Image.FromStream(sr);
                    var fileName = $"{userId}.{Guid.NewGuid()}.{s.FileName}";
                    return _blobProvider
                        .UploadStreamAsync(fileName, sr, s.ContentType, false, 60 * 24, token).ContinueWith(_ => fileName, token);
                }
            });
            var result = await Task.WhenAll(tasks).ConfigureAwait(false);
            return Ok(new
            {
                files = result
            });
        }
    }
}