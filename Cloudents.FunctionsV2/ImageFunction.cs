using System;
using Cloudents.Core;
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
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class ImageFunction
    {
        [FunctionName("ImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/{id}/{hash}")]
            HttpRequest req, string hash, long id,
            [Blob("spitball-files/files/{id}")] CloudBlobDirectory directory,
            [Inject] IDataProtectionProvider dataProtectProvider,
            ILogger log)
        {

            var dataProtector = dataProtectProvider.CreateProtector("image");
            if (string.IsNullOrEmpty(hash))
            {
                return new BadRequestResult();
            }

            var objectStr = dataProtector.Unprotect(hash);
            var properties = JsonConvert.DeserializeObject<ImageProperties>(objectStr);


            if (properties.Id != id)
            {
                return new BadRequestResult();
            }

            int.TryParse(req.Query["width"], out var width);
            int.TryParse(req.Query["height"], out var height);
            
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
                    Mode = ResizeMode.Crop,
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
                        context.HttpContext.Response.Headers.Add("Cache-Control", $"public, max-age={TimeConst.Year}");
                        image.SaveAsJpeg(stream);
                        image?.Dispose();
                        return Task.CompletedTask;
                    }
                    );
            }
        }

        public class ImageProperties
        {
            public long Id { get; set; }
            public int Page { get; set; }
            public bool Blur { get; set; }

        }
    }
}
