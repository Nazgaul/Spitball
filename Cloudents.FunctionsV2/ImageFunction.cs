using Cloudents.Core;
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using static Cloudents.Core.TimeConst;

namespace Cloudents.FunctionsV2
{
    public static class ImageFunction
    {
        private static readonly Dictionary<string, ImageExtensionConvert> Extension;
        static ImageFunction()
        {
            Extension = GetContainers().SelectMany(s => s.FileExtension, (convert, s) =>new {convert,s} )
                .ToDictionary(x=>x.s,y=>y.convert);
        }

        private static IEnumerable<ImageExtensionConvert> GetContainers()
        {
            // return Enum.GetValues(typeof(StorageContainer)).Cast<StorageContainer>();
            foreach (var field in typeof(ImageExtensionConvert).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.IsLiteral)
                {
                    continue;
                }
                yield return  (ImageExtensionConvert)field.GetValue(null);
            }
        }

        

        private class ImageExtensionConvert
        {
            protected bool Equals(ImageExtensionConvert other)
            {
                return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ImageExtensionConvert) obj);
            }

            public override int GetHashCode()
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
            }

            public static bool operator ==(ImageExtensionConvert left, ImageExtensionConvert right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(ImageExtensionConvert left, ImageExtensionConvert right)
            {
                return !Equals(left, right);
            }

            public string DefaultThumbnail { get; }
            public string Name { get; }

            public string[] FileExtension { get; }

            private ImageExtensionConvert(string defaultThumbnail, string[] extension,string name)
            {
                DefaultThumbnail = defaultThumbnail;
                FileExtension = extension;
                Name = name;
            }

            public static ImageExtensionConvert Text = new ImageExtensionConvert("Icons_720_txt.png", FormatDocumentExtensions.Text,nameof(Text));
            public static ImageExtensionConvert Excel = new ImageExtensionConvert("Icons_720_excel.png", FormatDocumentExtensions.Excel, nameof(Excel));
            public static ImageExtensionConvert Image = new ImageExtensionConvert("Icons_720_image.png", FormatDocumentExtensions.Image, nameof(Image));
            public static ImageExtensionConvert Pdf = new ImageExtensionConvert("Icons_720_pdf.png", FormatDocumentExtensions.Pdf, nameof(Pdf));
            public static ImageExtensionConvert PowerPoint = new ImageExtensionConvert("Icons_720_power.png", FormatDocumentExtensions.PowerPoint, nameof(PowerPoint));
            public static ImageExtensionConvert Tiff = new ImageExtensionConvert("Icons_720_txt.png", FormatDocumentExtensions.Tiff, nameof(Tiff));
            public static ImageExtensionConvert Word = new ImageExtensionConvert("Icons_720_doc.png", FormatDocumentExtensions.Word, nameof(Word));
        }



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

            var path = Path.GetExtension(blob.Name)?.ToLower();
            if (path != null && Extension.TryGetValue(path,out var val))
            {
                if (val != ImageExtensionConvert.Image)
                {
                    var blobPath = $"spitball-user/DefaultThumbnail/{val.DefaultThumbnail}";
                    blob = await binder.BindAsync<CloudBlockBlob>(new BlobAttribute(blobPath, FileAccess.Read),
                        token);
                    mode = ResizeMode.BoxPad;
                    //return new RedirectResult(blob2.Uri.AbsoluteUri);
                }
            }

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
