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

namespace Cloudents.FunctionsV2
{
    public static class ImageFunction
    {
        [FunctionName("ImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/{id}")] HttpRequest req,
            [Blob("spitball-files/files/{id}")] CloudBlobDirectory directory,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            int.TryParse(req.Query["width"], out var width);
            int.TryParse(req.Query["height"], out var height);
            int.TryParse(req.Query["page"], out var page);
            if (width == 0 || height == 0)
            {
                return new BadRequestResult();
            }

            var blob = directory.GetBlobReference($"preview-{page}.jpg");
            using (var sr = await blob.OpenReadAsync())
            {
                var image = Image.Load<Rgba32>(sr);
                image.Mutate(x => x.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Crop,
                    Size = new Size(width, height)
                }));
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
    }
}
