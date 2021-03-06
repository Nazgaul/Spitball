//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Extension;
//using Cloudents.FunctionsV2.Binders;
//using Cloudents.FunctionsV2.Di;
//using Cloudents.FunctionsV2.Extensions;
//using Cloudents.FunctionsV2.Services;
//using Cloudents.Query;
//using Cloudents.Query.Documents;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Extensions.Http;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Microsoft.WindowsAzure.Storage.Blob;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.PixelFormats;
//using SixLabors.ImageSharp.Processing;
//using Willezone.Azure.WebJobs.Extensions.DependencyInjection;


//namespace Cloudents.FunctionsV2
//{
//    public static class ShareDocumentImageFunction
//    {
//        [FunctionName("ShareDocumentImageFunction")]
//        public static async Task<IActionResult> RunAsync(
//            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/document/{id:long}")] HttpRequest req, long id,
//            [Blob("spitball/share-placeholder")] IEnumerable<CloudBlockBlob> directoryBlobs,
//            [HttpClientFactory] HttpClient client,
//            [Inject] IQueryBus queryBus,
//            ILogger log,
//            CancellationToken token)
//        {
//            ShareProfileImageFunction.InitData(directoryBlobs);
//            log.LogInformation("C# HTTP trigger function processed a request.");
            
//            bool.TryParse(req.Query["rtl"].ToString(), out var isRtl);
//            int.TryParse(req.Query["width"].ToString(), out var width);
//            int.TryParse(req.Query["height"].ToString(), out var height);

//            int.TryParse(req.Query["theme"], out var themeIndex);

//            var query = new ShareDocumentImageQuery(id);
//            var dbResult = await queryBus.QueryAsync(query, token);

//            if (dbResult is null)
//            {
//                return new BadRequestResult();
//            }

  
//            var themeBackground = $"share-placeholder/{dbResult.Type.ToString("G").ToLowerInvariant()}-bg-theme-{themeIndex}.jpg";

//            var bgBlob = ShareProfileImageFunction.Blobs.SingleOrDefault(s => s.Name == themeBackground) ??
//                         ShareProfileImageFunction.Blobs.Single(s => s.Name == $"share-placeholder/{dbResult.Type.ToString("G").ToLowerInvariant()}-bg-theme-1.jpg");

//            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port.GetValueOrDefault(443))
//            {
//                Path = "api/" + UrlConst.ImageFunctionDocumentRoute.Inject(new
//                {
//                    id,
//                })
//            };

//            using var logoImage = await ShareProfileImageFunction.GetImageFromBlobAsync("logo.png");

//            await using var bgBlobStream = await bgBlob.OpenReadAsync();
//            var image = Image.Load<Rgba32>(bgBlobStream);

//            image.Mutate(context =>
//            {
//                if (dbResult.Type == DocumentType.Video)
//                {
//                    // ReSharper disable AccessToDisposedClosure mutation happens right await
//                    logoImage.Mutate(m => m.ApplyProcessors(new ChangeLogoProcessorCreator()));
//                }
//                var logoPointX = 24;
//                if (isRtl)
//                {
//                    // ReSharper disable AccessToDisposedClosure mutation happens right await
//                    logoPointX = context.GetCurrentSize().Width - 24 - logoImage.Width;
//                }
//                // ReSharper disable AccessToDisposedClosure mutation happens right await
//                context.DrawImage(logoImage, new Point(logoPointX, 24),new GraphicsOptions());
//            });
//            if (dbResult.Type == DocumentType.Document)
//            {
              
//                uriBuilder.AddQuery(new
//                {
//                    width = 860,
//                    height = 471,
//                    anchorPosition = AnchorPositionMode.Top.ToString("G")
//                });


//                await using var documentPreviewStream = await GetImageStreamAsync(client, uriBuilder.Uri, token);
             
                
                
                
//                using var documentImage = Image.Load<Rgba32>(documentPreviewStream);

//                image.Mutate(context =>
//                {
//                    context.DrawText(dbResult.Name, 30, "#FFFFFF", new Size(860, 40), new Point(170, 66));
//                    context.DrawText(dbResult.CourseName, 26, "#ffffff", new Size(860, 40), new Point(170, 107));
//                    context.DrawImage(documentImage, new Point(170, 159), new GraphicsOptions());
//                });
//            }
//            else
//            {
//                uriBuilder.AddQuery(new
//                {
//                    width = 632,
//                    height = 356,
//                });

//                await using var documentPreviewStream = await GetImageStreamAsync(client, uriBuilder.Uri, token);

//                //await using var documentPreviewStream = await client.GetStreamAsync(uriBuilder.Uri);
//                using var documentImage = Image.Load<Rgba32>(documentPreviewStream);
//                using var playerImage = await ShareProfileImageFunction.GetImageFromBlobAsync("video-player.png");
//                image.Mutate(context =>
//                {
                    
//                    // ReSharper disable AccessToDisposedClosure mutation happens right await
//                    context.DrawImage(documentImage, new Point(305, 62), new GraphicsOptions());
//                    context.DrawImage(playerImage, new Point(520, 159), new GraphicsOptions());
//                    // ReSharper restore AccessToDisposedClosure

//                });
//            }
           

//            if (width > 0 && height > 0)
//            {
//                image.Mutate(m => m.Resize(width, height));
//            }

//            return new ImageResult(image, TimeSpan.FromDays(30));
//        }


//        private static async Task<Stream> GetImageStreamAsync(HttpClient client, Uri url,CancellationToken token)
//        {
//            var response =  await client.GetAsync(url, token);
//            //GetStream return stream without length image load need length - this return memory stream
//            return await response.Content.ReadAsStreamAsync();
//        }
//    }
//}
