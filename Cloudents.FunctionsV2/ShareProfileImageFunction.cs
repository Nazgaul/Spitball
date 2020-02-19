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
using Cloudents.FunctionsV2.Extensions;
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
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
  
    public static class ShareProfileImageFunction 
    {

        private static readonly Dictionary<Star, byte[]> StarDictionary = new Dictionary<Star, byte[]>();
        internal static List<CloudBlockBlob> Blobs;
        internal static readonly FontCollection FontCollection = new FontCollection();

        public const int SquareProfileImageDimension = 245;

        [FunctionName("ShareProfileImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/profile/{id:long}")] HttpRequest req, long id,
            [Blob("spitball/share-placeholder")] IEnumerable<CloudBlockBlob> directoryBlobs,
            [HttpClientFactory] HttpClient client,
            [Inject] IQueryBus queryBus,
            ILogger log,
            CancellationToken token)
        {
            await InitData(directoryBlobs);

            log.LogInformation("C# HTTP trigger function processed a request.");

            bool.TryParse(req.Query["rtl"].ToString(), out var isRtl);

            int.TryParse(req.Query["width"].ToString(), out var width);
            int.TryParse(req.Query["height"].ToString(), out var height);
            var query = new ShareProfileImageQuery(id);
            var dbResult = await queryBus.QueryAsync(query, token);

            if (dbResult?.Image is null)
            {
                return new BadRequestResult();
            }
            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port.GetValueOrDefault(443))
            {
                Path = "api/" + UrlConst.ImageFunctionUserRoute.Inject(new
                {
                    id,
                    file = dbResult.Image
                }),
            }.AddQuery(new
            {
                width = SquareProfileImageDimension,
                height = SquareProfileImageDimension
            });

            await using var profileImageStream = await client.GetStreamAsync(uriBuilder.Uri);
            var bgBlobName = $"share-placeholder/bg-profile-{(isRtl ? "rtl" : "ltr")}.jpg";
            var bgBlob = Blobs.Single(s => s.Name == bgBlobName);



            await using var bgBlobStream = await bgBlob.OpenReadAsync();
            var image = Image.Load<Rgba32>(bgBlobStream);

            using var profileImage = Image.Load<Rgba32>(profileImageStream);



            image.Mutate(context =>
            {
                DrawProfileImage(context, profileImage, isRtl);
                DrawProfileName(context, dbResult.Name, isRtl);
            });

            for (var i = 1; i <= 5; i++)
            {
                byte[] byteArr;
                if (dbResult.Rate >= i)
                {
                    byteArr = await GetStarAsync(Star.Full);
                }

                else if (Math.Abs(Math.Round((i - dbResult.Rate) * 2, MidpointRounding.AwayFromZero) / 2 - 0.5) < 0.01)
                {
                    byteArr = await GetStarAsync(Star.Half);
                }
                else
                {
                    byteArr = await GetStarAsync(Star.None);
                }

                var starImage = Image.Load(byteArr);

                var y = i;
                image.Mutate(context =>
                {
                    
                    const int marginBetweenState = 8;
                    var pointX = 132 + (y - 1) * (starImage.Width + marginBetweenState);
                    if (isRtl)
                    {
                        pointX = context.GetCurrentSize().Width - pointX - starImage.Width;
                    }

                    var point = new Point(pointX, 475);
                    context.DrawImage(starImage, point, GraphicsOptions.Default);
                });
            }


            var descriptionImage = await BuildDescriptionImage(dbResult.Description);


            var middleY = image.Height / 2 - descriptionImage.Height / 2;

            image.Mutate(x =>
            {
                var pointX = 493;
                if (isRtl)
                {
                    pointX = x.GetCurrentSize().Width - 493 - descriptionImage.Width;
                }

                x.DrawImage(descriptionImage, new Point(pointX, middleY), GraphicsOptions.Default);
            });

            if (width > 0 && height > 0)
            {
                image.Mutate(m=>m.Resize(width,height));
            }


            //image.Mutate(x=>x.DrawImage());
            return new ImageResult(image, TimeSpan.FromDays(365));

        }

        private static async Task<Image<Rgba32>> BuildDescriptionImage(string description)
        {
            await using var quoteSr = await Blobs.Single(w => w.Name == "share-placeholder/quote.png").OpenReadAsync();

            const int descriptionSize = 225;
            const int marginBetweenQuote = 28;
            var quoteImage = Image.Load(quoteSr);

            var descriptionImage = new Image<Rgba32>(675, descriptionSize + marginBetweenQuote + quoteImage.Height);
            descriptionImage.Mutate(context =>
            {
                var size = context.GetCurrentSize();
                var middle = size.Width / 2 - quoteImage.Width / 2;
                context.DrawImage(quoteImage, new Point(middle, 0), GraphicsOptions.Default);
                var font = FontCollection.CreateFont("assistant SemiBold", 38, FontStyle.Regular);

                var descriptionToDraw =
                    CropTextToFixToRectangle(font, description, new SizeF(size.Width, descriptionSize), true);
                //var rendererOptions = new RendererOptions(font)
                //{
                //    WrappingWidth = size.Width,
                //};
                //var textSize = TextMeasurer.Measure(descriptionToDraw, rendererOptions);

                var location = new PointF(0, quoteImage.Height + marginBetweenQuote);
                context.DrawTextWithHebrew(new TextGraphicsOptions()
                {
                    WrapTextWidth = size.Width,
                    HorizontalAlignment = HorizontalAlignment.Center,
                }, descriptionToDraw.text, font, Color.FromHex("43425d"), location);

                var endHeight = descriptionToDraw.size.Height + location.Y;
                if (endHeight < size.Height)
                {
                    context.Crop(size.Width, (int)(endHeight + 0.5));
                }
            });
            return descriptionImage;
        }

        private static void DrawProfileName(IImageProcessingContext context, string name, bool isRtl)
        {
            const float nameMaxWidth = 330f;
            var font = FontCollection.CreateFont("assistant", 32, FontStyle.Regular);
            var nameToDraw = CropTextToFixToRectangle(font, name, new SizeF(nameMaxWidth, 40f));

            var pointX = 79f;
            if (isRtl)
            {
                pointX = context.GetCurrentSize().Width - pointX - 330f;
            }

            context.DrawTextWithHebrew(
            new TextGraphicsOptions()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                WrapTextWidth = nameMaxWidth,
            },
            nameToDraw.text,
            font,
            Color.White,
            new PointF(pointX, 419));
        }

        internal static async Task<Image<Rgba32>> GetImageFromBlobAsync(string blobName)
        {
            var blobNameWithDirectory = $"share-placeholder/{blobName}";
            var blob = Blobs.Single(s => s.Name == blobNameWithDirectory);
            await using var stream = await blob.OpenReadAsync();
            return Image.Load<Rgba32>(stream);

        }

        internal static (string text,SizeF size) CropTextToFixToRectangle(Font font, string text, SizeF rectangle, bool threeDots = false)
        {
            var spanOfText = new Span<char>(text.ToCharArray());
            var rendererOptions = new RendererOptions(font)
            {
                WrappingWidth = rectangle.Width,
            };
            SizeF textSize;
            while (true)
            {
                textSize = TextMeasurer.Measure(spanOfText, rendererOptions);
                if (textSize.Height < rectangle.Height)
                {
                    break;
                }

                if (threeDots)
                {
                    spanOfText = spanOfText.Slice(0, spanOfText.LastIndexOf(' ') + 3);
                    spanOfText[^3..].Fill('.');
                }
                else
                {
                    spanOfText = spanOfText.Slice(0, spanOfText.LastIndexOf(' '));
                }
            }

            return (spanOfText.ToString(),textSize);
        }

        private static void DrawProfileImage(IImageProcessingContext context, Image<Rgba32> profileImage, bool isRtl)
        {
            const int offsetOfImage = 135;
            var pointX = offsetOfImage;
            if (isRtl)
            {
                pointX = context.GetCurrentSize().Width - offsetOfImage - profileImage.Width;
            }
            profileImage.Mutate(x => x.ApplyRoundedCorners(SquareProfileImageDimension / 2f));
            context.DrawImage(profileImage, new Point(pointX, 135), GraphicsOptions.Default);
        }

        internal static async Task InitData(IEnumerable<CloudBlockBlob> directoryBlobs)
        {
            if (Blobs is null)
            {
                Blobs = directoryBlobs.ToList();

                foreach (var fontBlob in Blobs.Where(w => w.Name.EndsWith(".ttf")))
                {
                    await using var fontStream = await fontBlob.OpenReadAsync();
                    FontCollection.Install(fontStream);
                }

                //foreach (var fontBlob in _blobs.Where(w => w.Name.EndsWith(".otf")))
                //{
                //    await using var fontStream = await fontBlob.OpenReadAsync();
                //    FontCollection.Install(fontStream);
                //}
            }
        }

        //public class ImageProperties
        //{
        //    public ImageProperties(string backgroundImage)
        //    {
        //        BackgroundImage = backgroundImage;
        //    }

        //    public string BackgroundImage { get; set; }
        //}

        //public static Dictionary<bool, ImageProperties> ImageDictionary2 = new Dictionary<bool, ImageProperties>()
        //{
        //    [true] = new ImageProperties("share-placeholder/bg-profile-rtl.jpg"),
        //    [false] = new ImageProperties("share-placeholder/bg-profile-ltr.jpg")
        //};




        // This method can be seen as an inline implementation of an `IImageProcessor`:
        // (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)



        private static async Task<byte[]> GetStarAsync(Star star)
        {
            if (StarDictionary.TryGetValue(star, out var v))
            {
                return v;
            }

            var blob = Blobs.Single(s => s.Name == star.BlobPath);
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

    }
}
