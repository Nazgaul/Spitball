using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.Di;
using Cloudents.FunctionsV2.Extensions;
using Cloudents.Query;
using Cloudents.Query.Documents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;


namespace Cloudents.FunctionsV2
{
    public static class ShareDocumentImageFunction
    {
        [FunctionName("ShareDocumentImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/document/{id:long}")] HttpRequest req, long id,
            [Blob("spitball/share-placeholder")] IEnumerable<CloudBlockBlob> directoryBlobs,
            [HttpClientFactory] HttpClient client,
            [Inject] IQueryBus queryBus,
            ILogger log,
            CancellationToken token)
        {
            await ShareProfileImageFunction.InitData(directoryBlobs);
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            bool.TryParse(req.Query["rtl"].ToString(), out var isRtl);
            int.TryParse(req.Query["width"].ToString(), out var width);
            int.TryParse(req.Query["height"].ToString(), out var height);

            int.TryParse(req.Query["theme"], out var themeIndex);

            var query = new ShareDocumentImageQuery(id);
            var dbResult = await queryBus.QueryAsync(query, token);

            if (dbResult is null)
            {
                return new BadRequestResult();
            }

  
            var themeBackground = $"share-placeholder/{dbResult.Type.ToString("G").ToLowerInvariant()}-bg-theme-{themeIndex}.jpg";

            var bgBlob = ShareProfileImageFunction.Blobs.SingleOrDefault(s => s.Name == themeBackground);
            if (bgBlob is null)
            {
                bgBlob = ShareProfileImageFunction.Blobs.Single(s => s.Name == $"share-placeholder/{dbResult.Type.ToString("G").ToLowerInvariant()}-bg-theme-1.jpg");
            }

            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port.GetValueOrDefault(443))
            {
                Path = "api/" + UrlConst.ImageFunctionDocumentRoute.Inject(new
                {
                    id,
                })
            };

            using var logoImage = await ShareProfileImageFunction.GetImageFromBlobAsync("logo.png");

            await using var bgBlobStream = await bgBlob.OpenReadAsync();
            var image = Image.Load<Rgba32>(bgBlobStream);

            image.Mutate(context =>
            {
                var logoPointX = 24;
                if (isRtl)
                {
                    logoPointX = context.GetCurrentSize().Width - 24 - logoImage.Width;
                }
                context.DrawImage(logoImage, new Point(logoPointX, 24), GraphicsOptions.Default);
            });
            if (dbResult.Type == DocumentType.Document)
            {
              
                uriBuilder.AddQuery(new
                {
                    width = 860,
                    height = 471,
                    anchorPosition = AnchorPositionMode.Top.ToString("G")
                });

                await using var documentPreviewStream = await client.GetStreamAsync(uriBuilder.Uri);
                using var documentImage = Image.Load<Rgba32>(documentPreviewStream);
                image.Mutate(context =>
                {

                    context.DrawText(dbResult.Name, 30, "#FFFFFF", new Size(860, 40), new Point(170, 66));

                    context.DrawText(dbResult.CourseName, 26, "#ffffff", new Size(860, 40), new Point(170, 107));

                    //var fontCourse =
                    //    ShareProfileImageFunction.FontCollection.CreateFont("assistant", 26, FontStyle.Regular);
                    //var courseToDraw =
                    //    ShareProfileImageFunction.CropTextToFixToRectangle(font, dbResult.CourseName,
                    //        new SizeF(860, 40f));
                    //context.DrawTextWithHebrew(
                    //    new TextGraphicsOptions()
                    //    {
                    //        HorizontalAlignment = HorizontalAlignment.Center,
                    //        WrapTextWidth = 860,
                    //    },
                    //    courseToDraw.text,
                    //    fontCourse,
                    //    Color.White,
                    //    new PointF(170, 107));

                    //context.DrawImage(documentImage, new Point(170, 159), GraphicsOptions.Default);

                });
            }
            else
            {
                uriBuilder.AddQuery(new
                {
                    width = 632,
                    height = 356,
                });

                await using var documentPreviewStream = await client.GetStreamAsync(uriBuilder.Uri);
                using var documentImage = Image.Load<Rgba32>(documentPreviewStream);
                using var playerImage = await ShareProfileImageFunction.GetImageFromBlobAsync("video-player.png");
                image.Mutate(context =>
                {
                    context.DrawImage(documentImage, new Point(305, 62), GraphicsOptions.Default);
                    context.DrawImage(playerImage, new Point(520, 159), GraphicsOptions.Default);

                });
            }

            if (width > 0 && height > 0)
            {
                image.Mutate(m => m.Resize(width, height));
            }

            return new ImageResult(image, TimeSpan.FromDays(365));
        }
    }
}
