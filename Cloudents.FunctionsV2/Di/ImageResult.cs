using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.Di
{

    internal sealed class ImageResult : FileResult, IDisposable
    {
        private readonly Image _image;
        private readonly TimeSpan _cacheLength;
        private readonly TypeToSave _typeToSave;

       


        public ImageResult(Image image, TimeSpan cacheLength, TypeToSave typeToSave = TypeToSave.Jpg) : base("image/jpg")
        {
            _image = image;
            _cacheLength = cacheLength;
            _typeToSave = typeToSave;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.Headers.Add("Cache-Control", $"public, max-age={_cacheLength.TotalSeconds}, s-max-age={_cacheLength.TotalSeconds}");
            if (_typeToSave == TypeToSave.Png)
            {
                context.HttpContext.Response.Headers.Add("content-type", "image/png");
                _image.SaveAsPng(context.HttpContext.Response.Body);
            }
            else
            {
                context.HttpContext.Response.Headers.Add("content-type", "image/jpg");
                _image.SaveAsJpeg(context.HttpContext.Response.Body);
            }
            return base.ExecuteResultAsync(context);
        }

        public void Dispose()
        {
            _image?.Dispose();
        }

        internal enum TypeToSave
        {
            Jpg,
            Png
        }
    }


}