using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Storage;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    //DO NOT ADD API CONTROLLER - UPLOAD WILL NOT WORK
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly string[] _supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };


        public UploadController(IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _blobProvider = blobProvider;
        }

        // GET
        [HttpPost("ask")]
        public async Task<UploadAskFileResponse> UploadFileAsync(UploadFileRequest model,
            [FromServices] UserManager<User> userManager,
            CancellationToken token)
        {
            var userId = userManager.GetUserId(User);

            var fileNames = new List<string>();
            foreach (var formFile in model.File)
            {
                if (!formFile.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("not an image");
                }

                var extension = Path.GetExtension(formFile.FileName);

                if (!_supportedImages.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("not an image");
                }

                using (var sr = formFile.OpenReadStream())
                {
                    Image.FromStream(sr);
                    var fileName = $"{userId}.{Guid.NewGuid()}.{formFile.FileName}";
                    await _blobProvider
                        .UploadStreamAsync(fileName, sr, formFile.ContentType, false, 60 * 24, token);

                    fileNames.Add(fileName);
                }
            }
            return new UploadAskFileResponse(fileNames);
        }
    }
}