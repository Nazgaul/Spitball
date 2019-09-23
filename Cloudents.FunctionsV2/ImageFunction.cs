using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
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
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Microsoft.WindowsAzure.Storage;
using SixLabors.Fonts;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using static Cloudents.Core.TimeConst;
using Path = System.IO.Path;

namespace Cloudents.FunctionsV2
{
    public static class ImageFunction
    {
        private static readonly Dictionary<string, string> ExtensionToDefaultImage;
        static ImageFunction()
        {
            //ExtensionToDefaultImage = GetContainers().SelectMany(s => s.FileExtension, (convert, s) => new { convert, s })
            //    .ToDictionary(x => x.s, y => y.convert);

            ExtensionToDefaultImage = FormatDocumentExtensions.GetTypes().Select(s => new
            {
                extensions = (string[]) s.GetValue(null),
                thumbnail = s.GetCustomAttribute<DefaultThumbnailAttribute>()?.Name ?? "doc-preview-empty.png"
            }).SelectMany(v =>v.extensions, (parent,child) => new
            {
                extension = child, parent.thumbnail
            }).ToDictionary(x => x.extension, z => z.thumbnail);
            //})
        }

        //private static IEnumerable<ImageExtensionConvert> GetContainers()
        //{
        //    // return Enum.GetValues(typeof(StorageContainer)).Cast<StorageContainer>();
        //    foreach (var field in typeof(ImageExtensionConvert).GetFields(BindingFlags.Public | BindingFlags.Static))
        //    {
        //        if (field.IsLiteral)
        //        {
        //            continue;
        //        }
        //        yield return (ImageExtensionConvert)field.GetValue(null);
        //    }
        //}


        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Using reflection")]

        //private class ImageExtensionConvert
        //{
        //    protected bool Equals(ImageExtensionConvert other)
        //    {
        //        return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        //    }

        //    public override bool Equals(object obj)
        //    {
        //        if (ReferenceEquals(null, obj)) return false;
        //        if (ReferenceEquals(this, obj)) return true;
        //        if (obj.GetType() != GetType()) return false;
        //        return Equals((ImageExtensionConvert)obj);
        //    }

        //    public override int GetHashCode()
        //    {
        //        return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
        //    }

        //    public static bool operator ==(ImageExtensionConvert left, ImageExtensionConvert right)
        //    {
        //        return Equals(left, right);
        //    }

        //    public static bool operator !=(ImageExtensionConvert left, ImageExtensionConvert right)
        //    {
        //        return !Equals(left, right);
        //    }

        //    public string DefaultThumbnail { get; }
        //    public string Name { get; }

        //    public string[] FileExtension { get; }

        //    private ImageExtensionConvert(string defaultThumbnail, string[] extension, string name)
        //    {
        //        DefaultThumbnail = defaultThumbnail;
        //        FileExtension = extension;
        //        Name = name;
        //    }

        //    public static ImageExtensionConvert Text = new ImageExtensionConvert("Icons_720_txt.png", FormatDocumentExtensions.Text, nameof(Text));
        //    public static ImageExtensionConvert Excel = new ImageExtensionConvert("Icons_720_excel.png", FormatDocumentExtensions.Excel, nameof(Excel));
        //    public static ImageExtensionConvert Image = new ImageExtensionConvert("Icons_720_image.png", FormatDocumentExtensions.Image, nameof(Image));
        //    public static ImageExtensionConvert Pdf = new ImageExtensionConvert("Icons_720_pdf.png", FormatDocumentExtensions.Pdf, nameof(Pdf));
        //    public static ImageExtensionConvert PowerPoint = new ImageExtensionConvert("Icons_720_power.png", FormatDocumentExtensions.PowerPoint, nameof(PowerPoint));
        //    public static ImageExtensionConvert Tiff = new ImageExtensionConvert("Icons_720_image.png", FormatDocumentExtensions.Tiff, nameof(Tiff));
        //    public static ImageExtensionConvert Word = new ImageExtensionConvert("Icons_720_doc.png", FormatDocumentExtensions.Word, nameof(Word));
        //}

        [FunctionName("ImageFunctionUser")]
        public static async Task<IActionResult> RunUserImageAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/user/{id}/{file}")]
            HttpRequest req, long id,string file,
            [Blob("spitball-user/profile/{id}/{file}")]CloudBlockBlob blob,
            Microsoft.Extensions.Logging.ILogger logger
        )
        {
            var regex = new Regex(@"[\d]*[.]\D{3,4}");
            var isBlob = regex.IsMatch(file);
            var mutation = ImageMutation.FromQueryString(req.Query);
            if (isBlob)
            {
                try
                {
                    using (var sr = await blob.OpenReadAsync())
                    {
                        return ProcessImage(sr,false, mutation);
                    }
                }
                catch (ImageFormatException ex)
                {
                    logger.LogError(ex, $"id: {id} file {file}");
                    return new RedirectResult(blob.Uri.AbsoluteUri);
                }
                catch (StorageException e)
                {
                    if (e.RequestInformation.HttpStatusCode == (int) HttpStatusCode.NotFound)
                    {
                        return GenerateImageFromName();
                    }

                    throw;
                }
            }
            return GenerateImageFromName();

            IActionResult GenerateImageFromName()
            {
                return new FileCallbackResult("image/jpg", (stream, context) =>
                {
                    context.HttpContext.Response.Headers.Add("Cache-Control",
                        $"public, max-age={Year}, s-max-age={Year}");
                    GenerateImageFromText(file, new Size(mutation.Width, mutation.Height), stream);

                    return Task.CompletedTask;
                });
            }
        }


        [FunctionName("ImageFunction")]
        public static async Task<IActionResult> Run(
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


            var blob = await binder.BindAsync<CloudBlockBlob>(new BlobAttribute(properties.Path, FileAccess.Read),
                token);

            var blobExtension = Path.GetExtension(blob.Name)?.ToLower();
            //if (path != null && !FormatDocumentExtensions.Image.Contains(path))
            if (blobExtension != null && ExtensionToDefaultImage.TryGetValue(blobExtension, out var val))
            {
                if (val != FormatDocumentExtensions.ImageDefaultImage)
                {
                    var blobPath = $"spitball-user/DefaultThumbnail/{val}";
                    blob = await binder.BindAsync<CloudBlockBlob>(new BlobAttribute(blobPath, FileAccess.Read),
                        token);
                    //mode = ResizeMode.BoxPad;
                }
            }

            mutation.BlurEffect = properties.Blur.GetValueOrDefault();
            //var mutation = new ImageMutation(width,height,mode,properties.Blur.GetValueOrDefault());
            try
            {
                using (var sr = await blob.OpenReadAsync())
                {
                    return ProcessImage(sr,false, mutation);
                }
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
                    using (t2.Result)
                    {
                        return ProcessImage(t2.Result,true, mutation);
                    }


                }

                throw;
            }
        }

        private static IActionResult ProcessImage(Stream sr, bool isDefault, ImageMutation mutation)
        {
            var image = Image.Load<Rgba32>(sr);
            image.Mutate(x => x.AutoOrient());
            image.Mutate(x => x.Resize(new ResizeOptions()
            {
                Mode = mutation.Mode,
                Size = new Size(mutation.Width, mutation.Height),
                Position = mutation.Position
            }));
            
            image.Mutate(x => x.BackgroundColor(Rgba32.White));
            switch (mutation.BlurEffect)
            {
                case ImageProperties.BlurEffect.None:
                    break;
                case ImageProperties.BlurEffect.Part:
                    //image.Mutate(x => x.BoxBlur(5));

                    image.Mutate(x => x.BoxBlur(5, new Rectangle(0, mutation.Height / 2, mutation.Width, mutation.Height / 2)));
                    break;
                case ImageProperties.BlurEffect.All:
                    image.Mutate(x => x.BoxBlur(5));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

           //
            return new FileCallbackResult("image/jpg", (stream, context) =>
            {
                if (!isDefault)
                {
                    context.HttpContext.Response.Headers.Add("Cache-Control",
                        $"public, max-age={Year}, s-max-age={Year}");
                }
                else 
                {
                    context.HttpContext.Response.Headers.Add("Cache-Control",
                        $"public, max-age={Hour}, s-max-age={Hour}");
                }

                image.SaveAsJpeg(stream);
                image?.Dispose();
                return Task.CompletedTask;
            });
        }

        private static readonly Rgba32[] Colors = {
            Rgba32.FromHex("64A9F8"),
            Rgba32.FromHex("ff9853"),
            Rgba32.FromHex("e775ce"),
            Rgba32.FromHex("10b2c1"),
            Rgba32.FromHex("848fa6"),
            Rgba32.FromHex("f36f6e"),
            Rgba32.FromHex("f76446"),
            Rgba32.FromHex("73b435"),
            Rgba32.FromHex("a55fff"),
            Rgba32.FromHex("a27c22"),
            Rgba32.FromHex("4faf61"),
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

        private static void GenerateImageFromText(string text, Size targetSize, Stream streamSaveLocation)
        {
            var fam = SystemFonts.Find("Arial");
            var font = new Font(fam, 100); // size doesn't matter too much as we will be scaling shortly anyway
            var style = new RendererOptions(font, 72); // again dpi doesn't overlay matter as this code genreates a vector

            // this is the important line, where we render the glyphs to a vector instead of directly to the image
            // this allows further vector manipulation (scaling, translating) etc without the expensive pixel operations.

            var glyphs = TextBuilder.GenerateGlyphs(GetTwoLetters(text), style);

            var widthScale = (targetSize.Width / glyphs.Bounds.Width);
            var heightScale = (targetSize.Height / glyphs.Bounds.Height);
            var minScale = Math.Min(widthScale, heightScale)*.5f;

            // scale so that it will fit exactly in image shape once rendered
            glyphs = glyphs.Scale(minScale);

            // move the vectorised glyph so that it touchs top and left edges 
            // could be tweeked to center horizontaly & vertically here
            glyphs = glyphs.Translate(-glyphs.Bounds.Location);
            glyphs = glyphs.Translate((targetSize.Width - glyphs.Bounds.Width) / 2, (targetSize.Height - glyphs.Bounds.Height) / 2);

            using (var img = new Image<Rgba32>(targetSize.Width, targetSize.Height))
            {
                var v = text.Select(Convert.ToInt32).Sum() % Colors.Length;
                img.Mutate(i=>i.BackgroundColor(Colors[v]));

                img.Mutate(i => i.Fill(new GraphicsOptions(true), Rgba32.White, glyphs));
                img.SaveAsJpeg(streamSaveLocation);
            }
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

            Enum.TryParse(query["anchorPosition"], true, out AnchorPositionMode position);

            if (width == 0)
            {
                width = 50;
            }

            if (height == 0)
            {
                height = 50;
            }

            return new ImageMutation(width,height,mode, position);
        }

        private ImageMutation(int width, int height, ResizeMode mode, AnchorPositionMode position)
        {
            Width = width;
            Height = height;
            Mode = mode;
            Position = position;
        }

        //public ImageMutation(int width, int height, ResizeMode mode, ImageProperties.BlurEffect blurEffect, AnchorPositionMode position)
        //{
        //    Width = width;
        //    Height = height;
        //    Mode = mode;
        //    BlurEffect = blurEffect;
        //    Position = position;
        //}

        public int Width { get;  }
        public int Height { get;  }

        public ResizeMode Mode { get;  }

        public ImageProperties.BlurEffect BlurEffect { get; set; }
        public AnchorPositionMode Position { get; }
    }
}
