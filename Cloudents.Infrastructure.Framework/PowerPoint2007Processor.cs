﻿using System;
using System.Threading;
using Aspose.Slides;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Aspose.Slides.Util;

namespace Cloudents.Infrastructure.Framework
{
    public class PowerPoint2007Processor :  IPreviewProvider2
    {
        public PowerPoint2007Processor()
        {
            SetLicense();
        }

        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }


      

        public static readonly string[] PowerPoint2007Extensions =
        {
          ".ppt",".pot", ".pps", ".pptx", ".potx", ".ppsx", ".odp", ".pptm"
        };

       

        private string ExtractStringFromPpt(Presentation ppt)
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
            catch (Exception ex)
            {
                return null;
            }
        }

        

        public async Task CreatePreviewFilesAsync(MemoryStream stream, Func<Stream, string, Task> callback, Func<string, Task> textCallback, CancellationToken token)
        {
            var pptx = new Presentation(stream);

            var t= ExtractStringFromPpt(pptx);
            if (t != null)
            {
                await textCallback(t);
            }

            for (int i = 0; i < pptx.Slides.Count; i++)
            {
                using (var img = pptx.Slides[0].GetThumbnail(1, 1))
                {
                    var ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Jpeg);
                    await callback(ms, $"{i}.jpg");
                }
            }

        }
    }
}
