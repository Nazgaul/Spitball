using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Di;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using static Cloudents.Core.TimeConst;

namespace Cloudents.FunctionsV2
{
    public static class ImageFunction
    {
        [FunctionName("ImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/{hash}")]
            HttpRequest req, string hash,
            IBinder binder,
            Microsoft.Extensions.Logging.ILogger logger,
            [Inject] IBinarySerializer serializer,
            CancellationToken token)
        {

            if (string.IsNullOrEmpty(hash))
            {
                return new BadRequestResult();
            }

            var hashBytes = Base64UrlTextEncoder.Decode(hash);

            var properties = serializer.Deserialize<ImageProperties>(hashBytes);
            int.TryParse(req.Query["width"], out var width);
            int.TryParse(req.Query["height"], out var height);
            if (!Enum.TryParse(req.Query["mode"], true, out ResizeMode mode))
            {
                mode = ResizeMode.Crop;
            }

            if (width == 0)
            {
                width = 50;
            }

            if (height == 0)
            {
                height = 50;
            }

            var blob = await binder.BindAsync<CloudBlockBlob>(new BlobAttribute(properties.Path, FileAccess.Read),
                token);

            using (var sr = await blob.OpenReadAsync())
            {
                try
                {
                    var image = Image.Load<Rgba32>(sr);

                    image.Mutate(x => x.Resize(new ResizeOptions()
                    {
                        Mode = mode,
                        Size = new Size(width, height)
                    }));
                    switch (properties.Blur.GetValueOrDefault())
                    {
                        case ImageProperties.BlurEffect.None:
                            break;
                        case ImageProperties.BlurEffect.Part:
                            image.Mutate(x => x.BoxBlur(5, new Rectangle(0, height / 2, width, height / 2)));
                            break;
                        case ImageProperties.BlurEffect.All:
                            image.Mutate(x => x.BoxBlur(5));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    image.Mutate(x => x.AutoOrient());
                    return new FileCallbackResult("image/jpg", (stream, context) =>
                    {
                        context.HttpContext.Response.Headers.Add("Cache-Control",
                            $"public, max-age={Year}, s-max-age={Year}");
                        image.SaveAsJpeg(stream);
                        image?.Dispose();
                        return Task.CompletedTask;
                    });

                }
                catch (ImageFormatException ex)
                {
                    logger.LogError(ex, hash);
                    return new RedirectResult(blob.Uri.AbsoluteUri);
                    //return new StatusCodeResult(500);
                }
            }
        }



    }
}
