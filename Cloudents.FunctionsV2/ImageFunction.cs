using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.FunctionsV2.Di;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Extensions;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using Path = System.IO.Path;

namespace Cloudents.FunctionsV2
{
    public static class ImageFunction
    {
        [FunctionName("ImageFunctionUser")]
        public static async Task<IActionResult> RunUserImageAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = UrlConst.ImageFunctionUserRoute)]
            HttpRequest req, long id, string file,
            [Blob("spitball-user/profile/{id}/{file}")]CloudBlockBlob blob,
            [Blob("spitball-user/DefaultThumbnail/cover-default.png")]CloudBlockBlob coverDefault,
            Microsoft.Extensions.Logging.ILogger logger
        )
        {
            var isCover = req.Query["type"].ToString().Equals("cover", StringComparison.OrdinalIgnoreCase);
            var regex = new Regex(@"[\d]*[.]\D{3,4}");
            var isBlob = regex.IsMatch(file);
            var mutation = ImageMutation.FromQueryString(req.Query);
            if (isBlob)
            {
                try
                {
                    mutation.CenterCords = await GetCenterCordsFromBlobAsync(blob);
                    await using var sr = await blob.OpenReadAsync();
                    var image = ProcessImage(sr, mutation);
                    return new ImageResult(image, TimeSpan.FromDays(365));
                }
                catch (ImageFormatException ex)
                {
                    logger.LogError(ex, $"id: {id} file {file}");
                    return new RedirectResult(blob.Uri.AbsoluteUri);
                }
                catch (StorageException e)
                {
                    if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
                    {
                        return await GenerateImageFromName();
                    }

                    throw;
                }
            }

            return await GenerateImageFromName();

            async Task<IActionResult> GenerateImageFromName()
            {
                if (isCover)
                {
                    await using var sr = await coverDefault.OpenReadAsync();
                    var imageCover = ProcessImage(sr, mutation);
                    return new ImageResult(imageCover, TimeSpan.FromDays(365));
                }
                var image = GenerateImageFromText(file, new Size(mutation.Width, mutation.Height));
                return new ImageResult(image, TimeSpan.FromDays(30));


            }
        }


        [FunctionName("ImageFunctionDocument")]
        public static async Task<IActionResult> RunDocumentImageAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = UrlConst.ImageFunctionDocumentRoute)]
            HttpRequest req, long id,
            IBinder binder,
            //collector search duplicate so i added some search 3 to solve this
            [Queue("generate-blob-preview")] IAsyncCollector<string> collectorSearch3,
            Microsoft.Extensions.Logging.ILogger logger,
            [Blob("spitball-files/files/{id}/preview-0.jpg")]CloudBlockBlob blob,
            CancellationToken token)
        {
            var mutation = ImageMutation.FromQueryString(req.Query);

            try
            {
                await using var sr = await blob.OpenReadAsync();
                var image = ProcessImage(sr, mutation);
                return new ImageResult(image, TimeSpan.FromDays(365));
            }
            catch (ImageFormatException ex)
            {
                logger.LogError(ex, id.ToString());
                return new RedirectResult(blob.Uri.AbsoluteUri);
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode != (int)HttpStatusCode.NotFound) throw;
                var t1 = collectorSearch3.AddAsync(id.ToString(), token);


                var directoryBlobs = await
                    binder.BindAsync<IEnumerable<ICloudBlob>>(new BlobAttribute($"spitball-files/files/{id}"), token);
                var blobPath = "spitball-user/DefaultThumbnail/doc-preview-empty.png";
                var fileBlob = directoryBlobs.FirstOrDefault(f => f.Name.Contains("/file-"));
                var blobExtension = Path.GetExtension(fileBlob?.Name)?.ToLower();

                if (blobExtension != null && FileTypesExtensions.FileExtensionsMapping.TryGetValue(blobExtension, out var val))
                {
                    blobPath = $"spitball-user/DefaultThumbnail/{val.DefaultThumbnail}";

                }

                var t2 = binder.BindAsync<Stream>(new BlobAttribute(blobPath, FileAccess.Read),
                    token);
                await Task.WhenAll(t1, t2);

                var result = await t2;
                await using (result)
                {
                    var image = ProcessImage(result, mutation);
                    return new ImageResult(image, TimeSpan.Zero);
                }
            }
        }

        [FunctionName("ImageFunction")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/{hash}")]
            HttpRequest req, string hash,
            IBinder binder,
            [Queue("generate-blob-preview")] IAsyncCollector<string> collectorSearch,
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

            var mutation = ImageMutation.FromQueryString(req.Query);


            var blob = await binder.BindAsync<CloudBlockBlob>(
                new BlobAttribute(properties.Path, FileAccess.Read),
                token);

            var blobExtension = Path.GetExtension(blob.Name)?.ToLower();
            if (blobExtension != null && FileTypesExtensions.FileExtensionsMapping.TryGetValue(blobExtension, out var val))
            {
                //This is for chat
                //if (blob.Container.Name == StorageContainer.Chat.Name && )
                if (val.DefaultThumbnail != FileTypesExtension.Image.DefaultThumbnail)
                {
                    var blobPath = $"spitball-user/DefaultThumbnail/{val.DefaultThumbnail}";
                    blob = await binder.BindAsync<CloudBlockBlob>(new BlobAttribute(blobPath, FileAccess.Read),
                        token);
                    mutation.Background = "#ffffff"; // apply white
                }
            }

            mutation.BlurEffect = properties.Blur.GetValueOrDefault();
            try
            {
                await using var sr = await blob.OpenReadAsync();
                mutation.CenterCords = await GetCenterCordsFromBlobAsync(blob);
                var image = ProcessImage(sr, mutation);
                return new ImageResult(image, TimeSpan.FromDays(365));
            }
            catch (ImageFormatException ex)
            {
                logger.LogError(ex, hash);
                return new RedirectResult(blob.Uri.AbsoluteUri);
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode != (int)HttpStatusCode.NotFound) throw;
                if (string.Equals(blob.Container.Name, StorageContainer.File.Name,
                    StringComparison.OrdinalIgnoreCase))
                {
                    var t1 = collectorSearch.AddAsync(
                        blob.Parent.Prefix.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last(), token);
                    const string blobPath = "spitball-user/DefaultThumbnail/doc-preview-empty.png";
                    var t2 = binder.BindAsync<Stream>(new BlobAttribute(blobPath, FileAccess.Read),
                        token);
                    await Task.WhenAll(t1, t2);
                    var result = await t2;
                    await using (result)
                    {
                        var image = ProcessImage(result, mutation);
                        return new ImageResult(image, TimeSpan.Zero);
                    }


                }

                throw;
            }
        }


        private static async Task<(float x, float y)?> GetCenterCordsFromBlobAsync(CloudBlob blob)
        {
            await blob.FetchAttributesAsync();
            if (blob.Metadata.TryGetValue("face", out var faceStr))

            {
                var arr = faceStr.Split(',').Select(s =>
                    new { valid = int.TryParse(s, out var v), result = v })
                    .Where(w => w.valid)
                    .ToArray();

                if (arr.Length != 2)
                {
                    return null;
                }

                return (arr[0].result, arr[1].result);// new float[] { arr[0].result, arr[1].result };
                //mutation.CenterCords = new float[] { arr[0].result, arr[1].result };
            }
            return null;
        }

        private static Image ProcessImage(Stream input, ImageMutation mutation)
        {
            var image = Image.Load<Rgba32>(input);
            image.Mutate(x => x.AutoOrient());

            image.Mutate(x =>
            {
                var v = new ResizeOptions()
                {
                    Mode = mutation.Mode,
                    Size = new Size(mutation.Width, mutation.Height),
                    Position = mutation.Position,
                   

                };
                if (mutation.CenterCords.HasValue)
                {
                    v.CenterCoordinates = new PointF(mutation.CenterCords.Value.x / image.Width, mutation.CenterCords.Value.y / image.Height);
                    //v.CenterCoordinates = new[]
                    //    {mutation.CenterCords.Value.x / image.Width, mutation.CenterCords.Value.y / image.Height};
                }
                //else
                //{
                //    v.CenterCoordinates = new PointF
                //    {
                //        0,0
                //    };
                //}



                x.Resize(v);

                if (mutation.RoundCorner > 0)
                {
                    x.ApplyRoundedCorners(mutation.RoundCorner);
                }
            });
            if (mutation.Background != null)
            {
                image.Mutate(x => x.BackgroundColor(Color.ParseHex(mutation.Background)));
            }

            //image.Mutate(x => x.BackgroundColor(Rgba32.White));
            switch (mutation.BlurEffect)
            {
                case ImageProperties.BlurEffect.None:
                    break;
                case ImageProperties.BlurEffect.All:
                    image.Mutate(x => x.BoxBlur(5));
                    break;
                default:
                    image.Mutate(x => x.BoxBlur(5));
                    break;
            }
            

            return image;

        }

        private static readonly Color[] Colors = {
            Color.ParseHex("64A9F8"),
            Color.ParseHex("ff9853"),
            Color.ParseHex("e775ce"),
            Color.ParseHex("10b2c1"),
            Color.ParseHex("848fa6"),
            Color.ParseHex("f36f6e"),
            Color.ParseHex("f76446"),
            Color.ParseHex("73b435"),
            Color.ParseHex("a55fff"),
            Color.ParseHex("a27c22"),
            Color.ParseHex("4faf61"),
        };

        private static string GetTwoLetters(string text)
        {
            var output = text.Truncate(2);
            if (RegEx.RtlLetters.IsMatch(output))
            {
                return new string(output.Reverse().ToArray());
            }

            return output;
        }

        private static Image GenerateImageFromText(string text, Size targetSize)
        {
            var fam = SystemFonts.Find("Calibri");
            var font = new Font(fam, 100); // size doesn't matter too much as we will be scaling shortly anyway
            var style = new RendererOptions(font, 72); // again dpi doesn't overlay matter as this code genreates a vector

            // this is the important line, where we render the glyphs to a vector instead of directly to the image
            // this allows further vector manipulation (scaling, translating) etc without the expensive pixel operations.

            var glyphs = TextBuilder.GenerateGlyphs(GetTwoLetters(text), style);

            var widthScale = (targetSize.Width / glyphs.Bounds.Width);
            var heightScale = (targetSize.Height / glyphs.Bounds.Height);
            var minScale = Math.Min(widthScale, heightScale) * .5f;

            // scale so that it will fit exactly in image shape once rendered
            glyphs = glyphs.Scale(minScale);

            // move the vectorised glyph so that it touchs top and left edges 
            // could be tweeked to center horizontaly & vertically here
            glyphs = glyphs.Translate(-glyphs.Bounds.Location);
            glyphs = glyphs.Translate((targetSize.Width - glyphs.Bounds.Width) / 2, (targetSize.Height - glyphs.Bounds.Height) / 2);

            var img = new Image<Rgba32>(targetSize.Width, targetSize.Height);

            var v = text.Select(Convert.ToInt32).Sum() % Colors.Length;
            img.Mutate(i => i.BackgroundColor(Colors[v]));
            img.Mutate(i => i.Fill(Color.White, glyphs));
            return img;
        }



    }

    public class ImageMutation
    {
        public static ImageMutation FromQueryString(IQueryCollection query)
        {
            int.TryParse(query["width"], out var width);
            int.TryParse(query["height"], out var height);
            if (!Enum.TryParse(query["mode"], true, out ResizeMode mode))
            {
                mode = ResizeMode.Crop;
            }

            int.TryParse(query["round"], out var round);
            Enum.TryParse(query["anchorPosition"], true, out AnchorPositionMode position);

            if (width == 0)
            {
                width = 50;
            }

            if (height == 0)
            {
                height = 50;
            }

            //var centerCords = query["center"].ToArray()?.Select(s => float.Parse(s));

            return new ImageMutation(width, height, mode, position, round);
        }

        private ImageMutation(int width, int height, 
            ResizeMode mode, AnchorPositionMode position, int roundCorner)
        {
            Width = width;
            Height = height;
            Mode = mode;
            Position = position;
            RoundCorner = roundCorner;
            //CenterCords = centerCords ?? Array.Empty<float>();

        }

        public (float x,float y)? CenterCords { get; set; }
        public int Width { get; }
        public int Height { get; }

        public int RoundCorner { get; }

        public ResizeMode Mode { get; }

        public ImageProperties.BlurEffect BlurEffect { get; set; }
        public AnchorPositionMode Position { get; }
        public string? Background { get; set; }
    }
}
