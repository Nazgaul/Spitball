﻿using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Text;
using Aspose.Pdf.Text.TextOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Framework
{
    public class PdfProcessor : IPreviewProvider2 //: Processor, IPreviewProvider
    {

        public PdfProcessor()
        {
            using (var sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("Cloudents.Infrastructure.Framework.Aspose.Total.lic"))
            {
                var license = new License();
                license.SetLicense(sr);
            }
        }


        public async Task ProcessFilesAsync(Stream stream,
            Func<Stream, string, Task> pagePreviewCallback,
            Func<string, int, Task> metaCallback,
            CancellationToken token)
        {
            var pdf = new Document(stream);
            var txt = ExtractPdfText(pdf);

            await metaCallback(txt, pdf.Pages.Count - 1);
            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);
            var t = new List<Task>();
            for (var j = 1; j < pdf.Pages.Count; j++)
            {
                var page = pdf.Pages[j];
                var ms = new MemoryStream();
                jpegDevice.Process(page, ms);
                t.Add(pagePreviewCallback(ms, $"{j - 1}.jpg").ContinueWith(_ => ms.Dispose(), token));

            }
            await Task.WhenAll(t);
        }

        private static string ExtractPdfText(Document doc)
        {
            var textAbsorber = new TextAbsorber
            {
                ExtractionOptions = new TextExtractionOptions(TextExtractionOptions.TextFormattingMode.Pure)
            };
            for (var i = 1; i <= Math.Min(doc.Pages.Count, 20); i++)
            {
                try
                {
                    doc.Pages[i].Accept(textAbsorber);
                }
                catch (IndexOutOfRangeException)
                {

                }
                catch (ArgumentException)
                {
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            return textAbsorber.Text;

        }

        public static readonly string[] Extensions = { ".pdf" };

    }
}
