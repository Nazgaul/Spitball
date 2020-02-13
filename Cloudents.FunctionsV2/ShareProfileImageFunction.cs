using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.Di;
using Cloudents.Query;
using Cloudents.Query.Tutor;
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
using SixLabors.Shapes;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class ShareProfileImageFunction
    {

        private static readonly Dictionary<Star, byte[]> StarDictionary = new Dictionary<Star, byte[]>();
        private static List<CloudBlockBlob> _blobs;
        private static readonly FontCollection _fontCollection = new FontCollection();


        [FunctionName("ShareProfileImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/profile/{id:long}")] HttpRequest req, long id,
            [Blob("spitball/share-placeholder")] IEnumerable<CloudBlockBlob> directoryBlobs,

            [HttpClientFactory] HttpClient client,
            [Inject] IQueryBus queryBus,
            ILogger log,
            CancellationToken token)
        {
            if (_blobs is null)
            {
                _blobs = directoryBlobs.ToList();

                foreach (var fontBlob in _blobs.Where(w => w.Name.EndsWith(".ttf")))
                {
                    await using var fontStream = await fontBlob.OpenReadAsync();
                    _fontCollection.Install(fontStream);

                }
            }

            log.LogInformation("C# HTTP trigger function processed a request.");

            var query = new ShareProfileImageQuery(id);
            var result = await queryBus.QueryAsync(query, token);

            if (result.Image is null)
            {
                return new BadRequestResult();
            }


            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port.Value)
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
            var bgBlob = $"share-placeholder/bg-profile-ltr.jpg";
            var blob = _blobs.Single(s => s.Name == bgBlob);


            await using var bgBlobStream = await blob.OpenReadAsync();
            var image = Image.Load<Rgba32>(bgBlobStream);

            using var profileImage = Image.Load<Rgba32>(profileImageStream);
            profileImage.Mutate(x => x.ApplyRoundedCorners(245f / 2));


            image.Mutate(context =>
            {
                context.DrawImage(profileImage, new Point(148, 135), GraphicsOptions.Default);
                context.DrawText(
                    new TextGraphicsOptions()
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        WrapTextWidth = 330f,
                    },
                    result.Name,
                    _fontCollection.CreateFont("open sans", 32, FontStyle.Regular),
                    Color.White,
                    new PointF(105f, 423));
            });

            for (var i = 1; i <= 5; i++)
            {
                byte[] byteArr;
                if (result.Rate >= i)
                {
                    byteArr = await GetStarAsync(Star.Full);
                }

                else if (Math.Abs(Math.Round((i - result.Rate) * 2, MidpointRounding.AwayFromZero) / 2 - 0.5) < 0.01)
                {
                    byteArr = await GetStarAsync(Star.Half);
                }
                else
                {
                    byteArr = await GetStarAsync(Star.None);
                }

                var starImage = Image.Load(byteArr);

                //var z = i - 1;
                const int marginBetweenState = 8;
                const int stateWidth = 43;
                var point = new Point(148 + (i - 1) * (stateWidth + marginBetweenState), 475);
                image.Mutate(x => x.DrawImage(starImage, point, GraphicsOptions.Default));
            }


            await using var quoteSr = await _blobs.Single(w => w.Name == "share-placeholder/quote.png").OpenReadAsync();

            const int descriptionSize = 255;
            const int marginBetweenQuote = 28;
            var quoteImage = Image.Load(quoteSr);

            var descriptionImage = new Image<Rgba32>(675, descriptionSize + marginBetweenQuote + quoteImage.Height);
            result.Description =
                "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
            descriptionImage.Mutate(context =>
            {
                ReadOnlySpan<char> description = result.Description;
                context.BackgroundColor(Color.Bisque);

                var size = context.GetCurrentSize();

                var middle = size.Width / 2 - quoteImage.Width / 2;
                context.DrawImage(quoteImage, new Point(middle, 0), GraphicsOptions.Default);
                var font = _fontCollection.CreateFont("Open Sans Semibold", 38, FontStyle.Bold);

                var rendererOptions = new RendererOptions(font)
                {
                    WrappingWidth = size.Width,
                };
                SizeF textSize;
                while (true)
                {
                    textSize = TextMeasurer.Measure(description, rendererOptions);
                    if (textSize.Height < descriptionSize)
                    {
                        break;
                    }
                    description = description.Slice(0, description.LastIndexOf(' '));
                }

                var location = new PointF(0, quoteImage.Height + marginBetweenQuote);

                context.DrawText(new TextGraphicsOptions()
                {
                    WrapTextWidth = size.Width,
                    HorizontalAlignment = HorizontalAlignment.Center
                }, description.ToString(), font, Color.FromHex("43425d"), location);

                var endHeight = textSize.Height + location.Y;
                if (endHeight < size.Height)
                {
                    context.Crop(size.Width, (int)endHeight);
                }
            });


            var middleY = image.Height / 2 - descriptionImage.Height / 2;

            image.Mutate(x => x.DrawImage(descriptionImage, new Point(493, middleY), GraphicsOptions.Default));



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


        private static async Task<byte[]> GetStarAsync(Star star)
        {
            if (StarDictionary.TryGetValue(star, out var v))
            {
                return v;
            }

            var blob = _blobs.Single(s => s.Name == star.BlobPath);
            //using (var ms = new MemoryStream())
            //{
            //    blob.DownloadToStreamAsync(ms);
            //}
            var bytes = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(bytes, 0);

            StarDictionary[star] = bytes;

            return bytes;
        }
    }

    public class Star : Enumeration
    {
        public string BlobPath { get; }
        private Star(int id, string name, string blobPath) : base(id, name)
        {
            BlobPath = $"share-placeholder/{blobPath}";

        }

        public static readonly Star Full = new Star(1, "Full", "star-full.png");
        public static readonly Star Half = new Star(2, "Half", "star-half.png");
        public static readonly Star None = new Star(3, "Empty", "star-empty.png");
        //None,
        //Half,
        //Full

    }
}
