using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.Di
{

    public sealed class ImageResult : FileResult, IDisposable
    {
        private readonly Image _image;
        private readonly TimeSpan _cacheLength;

        public ImageResult(Image image, TimeSpan cacheLength) : base("image/jpg")
        {
            _image = image;
            _cacheLength = cacheLength;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers.Add("content-type", "image/jpg");
            context.HttpContext.Response.Headers.Add("Cache-Control", $"public, max-age={_cacheLength.TotalSeconds}, s-max-age={_cacheLength.TotalSeconds}");
            _image.SaveAsJpeg(context.HttpContext.Response.Body);
            return base.ExecuteResultAsync(context);
        }

        public void Dispose()
        {
            _image?.Dispose();
        }
    }
}