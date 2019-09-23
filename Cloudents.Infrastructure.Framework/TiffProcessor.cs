using Aspose.Imaging;
using Aspose.Imaging.FileFormats.Jpeg;
using Aspose.Imaging.FileFormats.Tiff;
using Aspose.Imaging.ImageOptions;
using Aspose.Imaging.Sources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;

namespace Cloudents.Infrastructure.Framework
{
    public class TiffProcessor : IPreviewProvider, IDisposable
    {
        public TiffProcessor()

        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }

        private TiffImage _image;
        public void Init(Func<Stream> stream)
        {
            _image = (TiffImage)Image.Load(stream());
        }

        public void Init(Func<string> stream)
        {
            _image = (TiffImage)Image.Load(stream());
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {
            return (null, _image.Frames.Length);
        }

        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback, CancellationToken token)
        {
            var jpgCreateOptions = new JpegOptions();

            var diff = Enumerable.Range(0, _image.Frames.Length);
            diff = diff.Except(previewDelta);

            var t = new List<Task>();
            foreach (var item in diff)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                _image.ActiveFrame = _image.Frames[item];// tiffFrame;
                var pixels = _image.LoadPixels(_image.Bounds);
                //bmpCreateOptions as FileCreateSource saved
                var ms = new MemoryStream();
                jpgCreateOptions.Source = new StreamSource(ms);
                using (var jpgImage =
                    (JpegImage)Image.Create(jpgCreateOptions, _image.Width, _image.Height))
                {
                    //TiffFrame
                    jpgImage.SavePixels(_image.Bounds, pixels);
                    jpgImage.Save();
                }

                var task = pagePreviewCallback(ms, $"{item}.jpg").ContinueWith(_ => ms.Dispose(), token);
                t.Add(task);
            }
            await Task.WhenAll(t);
        }

        public static readonly string[] Extensions = FormatDocumentExtensions.Tiff;


        public void Dispose()
        {
            _image?.Dispose();
        }

        //public void Init(Func<string> path)
        //{
        //    _image = (TiffImage)Image.Load(path());
        //}
    }
}
