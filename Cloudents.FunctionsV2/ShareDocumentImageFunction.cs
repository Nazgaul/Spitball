using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
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
using Newtonsoft.Json;
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

            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port.GetValueOrDefault(443))
            {
                Path = "api/" + UrlConst.ImageFunctionDocumentRoute.Inject(new
                {
                    id,
                }),
            }.AddQuery(new
            {
                width = 860,
                height = 471,
                anchorPosition = AnchorPositionMode.Top.ToString("G")
            });

            await using var documentPreviewStream = await client.GetStreamAsync(uriBuilder.Uri);
            using var documentImage = Image.Load<Rgba32>(documentPreviewStream);
            var themeBackground = $"share-placeholder/document-bg-theme-{themeIndex}.jpg";

            var bgBlob = ShareProfileImageFunction._blobs.SingleOrDefault(s => s.Name == themeBackground);
            if (bgBlob is null)
            {
                bgBlob = ShareProfileImageFunction._blobs.SingleOrDefault(s => s.Name == "share-placeholder/document-bg-theme-1.jpg");
            }


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
                // var size = context.GetCurrentSize();
                //var middleX = size.Width / 2 + documentImage.Width / 2;
                var font = ShareProfileImageFunction.FontCollection.CreateFont("assistant", 30, FontStyle.Regular);
                var nameToDraw = ShareProfileImageFunction.CropTextToFixToRectangle(font, dbResult.Name, new SizeF(860, 40f));
                context.DrawTextWithHebrew(
                    new TextGraphicsOptions()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        WrapTextWidth = 860,
                    },
                    nameToDraw,
                    font,
                    Color.White,
                    new PointF(170, 66));

                var fontCourse = ShareProfileImageFunction.FontCollection.CreateFont("assistant", 26, FontStyle.Regular);
                var courseToDraw = ShareProfileImageFunction.CropTextToFixToRectangle(font, dbResult.CourseName, new SizeF(860, 40f));
                context.DrawTextWithHebrew(
                    new TextGraphicsOptions()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        WrapTextWidth = 860,
                    },
                    courseToDraw,
                    fontCourse,
                    Color.White,
                    new PointF(170, 107));

                context.DrawImage(documentImage, new Point(170, 159), GraphicsOptions.Default);

            });

            if (width > 0 && height > 0)
            {
                image.Mutate(m => m.Resize(width, height));
            }

            return new ImageResult(image, TimeSpan.FromDays(365));
        }
    }
}
