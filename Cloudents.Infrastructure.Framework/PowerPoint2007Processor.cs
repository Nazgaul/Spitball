﻿using Aspose.Slides;
using Aspose.Slides.Util;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public class PowerPoint2007Processor : IPreviewProvider2, IDisposable
    {
        public PowerPoint2007Processor()
        {
            SetLicense();
        }

        private static void SetLicense()
        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }




        public static readonly string[] Extensions =
        {
            ".ppt", ".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm"
        };



        private static string ExtractStringFromPpt(Presentation ppt)
        {
            try
            {
                var sb = new StringBuilder();



                //Loop through the Array of TextFrames
                foreach (ITextFrame t in SlideUtil.GetAllTextFrames(ppt, false))
                foreach (var para in t.Paragraphs)
                    //Loop through portions in the current Paragraph
                foreach (var port in para.Portions)
                {
                    //Display text in the current portion
                    sb.Append(port.Text);
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Presentation _pptx;

        public void Init(Stream stream)
        {
            _pptx = new Presentation(stream);
            
        }

        public (string text, int pagesCount) ExtractMetaContent()
        {
            var txt = ExtractStringFromPpt(_pptx);
            return (txt, _pptx.Slides.Count);
        }

       

        public async Task ProcessFilesAsync(IEnumerable<int> previewDelta, Func<Stream, string, Task> pagePreviewCallback,
            CancellationToken token)
        {
            var tasksList = new List<Task>();

            var diff = Enumerable.Range(0, _pptx.Slides.Count);
            diff = diff.Except(previewDelta);

            foreach (var item in diff)
            {
                using (var img = _pptx.Slides[0].GetThumbnail(1, 1))
                {
                    var ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Jpeg);
                    tasksList.Add(pagePreviewCallback(ms, $"{item}.jpg").ContinueWith(_ => ms.Dispose(), token));
                }
            }
            await Task.WhenAll(tasksList);

        }

        public void Dispose()
        {
            _pptx?.Dispose();
        }
    }
}
