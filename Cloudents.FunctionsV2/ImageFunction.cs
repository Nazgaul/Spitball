using Cloudents.FunctionsV2.Di;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Microsoft.AspNetCore.WebUtilities;
using static Cloudents.Core.TimeConst;

namespace Cloudents.FunctionsV2
{
    public static class ImageFunction
    {
        [FunctionName("ImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/{id}/{hash}")]
            HttpRequest req, string hash, long id,
            [Blob("spitball-files/files/{id}")] CloudBlobDirectory directory,
            ILogger log)
        {

            if (string.IsNullOrEmpty(hash))
            {
                return new BadRequestResult();
            }

            var hashBytes = Base64UrlTextEncoder.Decode(hash);
            var properties = ImageProperties.Decrypt(hashBytes);


            if (properties.Id != id)
            {
                return new BadRequestResult();
            }

            int.TryParse(req.Query["width"], out var width);
            int.TryParse(req.Query["height"], out var height);
            if (!Enum.TryParse(req.Query["mode"], true, out ResizeMode mode))
            {
                mode = ResizeMode.Crop;
            }
            if (width == 0 || height == 0)
            {
                return new BadRequestResult();
            }

            var blob = directory.GetBlobReference($"preview-{properties.Page}.jpg");
            using (var sr = await blob.OpenReadAsync())
            {
                var image = Image.Load<Rgba32>(sr);
                image.Mutate(x => x.Resize(new ResizeOptions()
                {
                    Mode = mode,
                    Size = new Size(width, height)
                }));
                if (properties.Blur)
                {
                    if (properties.Page == 0)
                    {
                        image.Mutate(x => x.BoxBlur(5, new Rectangle(0, height / 2, width, height / 2)));
                    }
                    else
                    {
                        image.Mutate(x => x.BoxBlur(5));

                    }
                }

                return new FileCallbackResult("image/jpg", (stream, context) =>
                    {
                        context.HttpContext.Response.Headers.Add("Cache-Control", $"public, max-age={Year}, s-max-age={Year}");
                        image.SaveAsJpeg(stream);
                        image?.Dispose();
                        return Task.CompletedTask;
                    }
                );
            }
        }
    }
}
