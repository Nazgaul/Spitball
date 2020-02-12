using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.Di;
using Cloudents.FunctionsV2.Services;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class ShareProfileImageFunction
    {
        [FunctionName("ShareProfileImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/profile/{id:long}")] HttpRequest req, long id,
            [Blob("spitball/share-placeholder")] IEnumerable<CloudBlockBlob> directoryBlobs,
            [HttpClientFactory] HttpClient client,
            [Inject] IQueryBus queryBus,
            ILogger log,
            CancellationToken token)
        {
            //if (backgroundStream == null) throw new ArgumentNullException(nameof(backgroundStream));
            log.LogInformation("C# HTTP trigger function processed a request.");

            var query = new ShareProfileImageQuery(id);
            var result = await queryBus.QueryAsync(query, token);

            if (result.Image is null)
            {
                return new BadRequestResult();
            }
            

            var uriBuilder = new UriBuilder(req.Scheme,req.Host.Host,req.Host.Port.Value)
            {
                Path = "api/" + UrlConst.ImageFunctionUserRoute.Inject(new
                {
                    id,
                    file = result.Image
                }),
            }.AddQuery(new
            {
                width = 245,
                height = 245
            });

            await using var profileImageStream = await client.GetStreamAsync(uriBuilder.Uri);
            //foreach (var cloudBlockBlob in directoryBlobs)
            //{

            //}
            //var v = directoryBlobs.ToList();

            // var bgBlob = $"share-placeholder/bg-profile-{(result.Country.MainLanguage.Info.TextInfo.IsRightToLeft ? "rtl" : "ltr")}.jpg";
            var bgBlob = $"share-placeholder/bg-profile-ltr.jpg";
            var blob = directoryBlobs.Single(s => s.Name == bgBlob);

            await using var bgBlobStream = await blob.OpenReadAsync();
            var image = Image.Load<Rgba32>(bgBlobStream);

            using var profileImage = Image.Load<Rgba32>(profileImageStream);
           
            profileImage.Mutate(x=>x.ApplyRoundedCorners(245f/2));
            image.Mutate(x=>x.DrawImage(profileImage, new Point(148,135),GraphicsOptions.Default));
            //using (var logoImage = Image.Load<Rgba32>(logoStream))
            //{


            //    image.Mutate(x => x.DrawImage(logoImage, new Point(139, 39), GraphicsOptions.Default));
            //}
            //if (result.Country == Country.Israel.Name)
            //{
            //    image.Mutate(x => x.Flip(FlipMode.Horizontal));
            //}

            //image.Mutate(x=>x.DrawImage());
            return new ImageResult(image, TimeSpan.Zero);

        }

        public class ImageProperties
        {
            public ImageProperties(string backgroundImage)
            {
                BackgroundImage = backgroundImage;
            }

            public string BackgroundImage { get; set; }
        }

        public static Dictionary<bool, ImageProperties> ImageDictionary2 = new Dictionary<bool, ImageProperties>()
        {
            [true] = new ImageProperties("share-placeholder/bg-profile-rtl.jpg"),
            [false] = new ImageProperties("share-placeholder/bg-profile-ltr.jpg")
        };




        // This method can be seen as an inline implementation of an `IImageProcessor`:
        // (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
        private static IImageProcessingContext ApplyRoundedCorners(this IImageProcessingContext ctx, float cornerRadius)
        {
            Size size = ctx.GetCurrentSize();
            IPathCollection corners = BuildCorners(size.Width, size.Height, cornerRadius);

            var graphicOptions = new GraphicsOptions(true)
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
            };
            // mutating in here as we already have a cloned original
            // use any color (not Transparent), so the corners will be clipped
            return ctx.Fill(graphicOptions, Rgba32.LimeGreen, corners);
        }

        private static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the original around the center of the image

            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            // move it across the width of the image - the width of the shape
            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }

    }
}
