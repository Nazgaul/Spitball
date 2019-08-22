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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using static Cloudents.Core.TimeConst;

namespace Cloudents.FunctionsV2
{
    public static class ImageFunction
    {
        private static readonly Dictionary<string, ImageExtensionConvert> Extension;
        static ImageFunction()
        {
            Extension = GetContainers().SelectMany(s => s.FileExtension, (convert, s) => new { convert, s })
                .ToDictionary(x => x.s, y => y.convert);
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
                yield return (ImageExtensionConvert)field.GetValue(null);
            }
        }



        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Using reflection")]
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
                if (obj.GetType() != GetType()) return false;
                return Equals((ImageExtensionConvert)obj);
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

            private ImageExtensionConvert(string defaultThumbnail, string[] extension, string name)
            {
                DefaultThumbnail = defaultThumbnail;
                FileExtension = extension;
                Name = name;
            }

            public static ImageExtensionConvert Text = new ImageExtensionConvert("Icons_720_txt.png", FormatDocumentExtensions.Text, nameof(Text));
            public static ImageExtensionConvert Excel = new ImageExtensionConvert("Icons_720_excel.png", FormatDocumentExtensions.Excel, nameof(Excel));
            public static ImageExtensionConvert Image = new ImageExtensionConvert("Icons_720_image.png", FormatDocumentExtensions.Image, nameof(Image));
            public static ImageExtensionConvert Pdf = new ImageExtensionConvert("Icons_720_pdf.png", FormatDocumentExtensions.Pdf, nameof(Pdf));
            public static ImageExtensionConvert PowerPoint = new ImageExtensionConvert("Icons_720_power.png", FormatDocumentExtensions.PowerPoint, nameof(PowerPoint));
            public static ImageExtensionConvert Tiff = new ImageExtensionConvert("Icons_720_image.png", FormatDocumentExtensions.Tiff, nameof(Tiff));
            public static ImageExtensionConvert Word = new ImageExtensionConvert("Icons_720_doc.png", FormatDocumentExtensions.Word, nameof(Word));
        }

        [FunctionName("ImageFunctionUser")]
        public static async Task<IActionResult> RunUserImageAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "image/user/{id}/{file}")]
            HttpRequest req, long id,string file,
            [Blob("spitball-user/profile/{id}/{file}")]CloudBlockBlob blob,
            Microsoft.Extensions.Logging.ILogger logger
        )
        {
            var mutation = ImageMutation.FromQueryString(req.Query);
            try
            {
                using (var sr = await blob.OpenReadAsync())
                {
                    return ProcessImage(sr, mutation);
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
                    return new NotFoundResult();
                }

                throw;
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

            var path = Path.GetExtension(blob.Name)?.ToLower();
            if (path != null && Extension.TryGetValue(path, out var val))
            {
                if (val != ImageExtensionConvert.Image)
                {
                    var blobPath = $"spitball-user/DefaultThumbnail/{val.DefaultThumbnail}";
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
                    return ProcessImage(sr, mutation);
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
                        return ProcessImage(t2.Result, mutation);
                    }


                }

                throw;
            }
        }

        private static IActionResult ProcessImage(Stream sr, ImageMutation mutation)
        {
            var image = Image.Load<Rgba32>(sr);
            image.Mutate(x => x.AutoOrient());
            image.Mutate(x => x.Resize(new ResizeOptions()
            {
                Mode = mutation.Mode,
                Size = new Size(mutation.Width, mutation.Height),
                Position = mutation.Position
            }));

            switch (mutation.BlurEffect)
            {
                case ImageProperties.BlurEffect.None:
                    break;
                case ImageProperties.BlurEffect.Part:
                    image.Mutate(x => x.BoxBlur(5, new Rectangle(0, mutation.Height / 2, mutation.Width, mutation.Height / 2)));
                    break;
                case ImageProperties.BlurEffect.All:
                    image.Mutate(x => x.BoxBlur(5));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            image.Mutate(x => x.BackgroundColor(Rgba32.White));
            return new FileCallbackResult("image/jpg", (stream, context) =>
            {
                context.HttpContext.Response.Headers.Add("Cache-Control",
                    $"public, max-age={Year}, s-max-age={Year}");
                image.SaveAsJpeg(stream);
                image?.Dispose();
                return Task.CompletedTask;
            });
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
