using ImageResizer;
using ImageResizer.Plugins;
using ImageResizer.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImageResizer.Configuration;
using ImageResizer.Resizing;

namespace Cloudents.Infrastructure.Framework
{
    public class ImageProcessor : IPreviewProvider2
    {
        public static readonly string[] Extensions = { ".jpg", ".gif", ".png", ".jpeg", ".bmp" };

        public async Task ProcessFilesAsync(Stream stream,
            Func<Stream, string, Task> pagePreviewCallback,

            Func<string, int, Task> metaCallback,
            CancellationToken token)
        {


            //TODO: we can do it faster
            //new Gradient().Install(
            //    Config.Current);

            await metaCallback(null, 1);
            using (var ms = new MemoryStream())
            {
                var settings2 = new ResizeSettings
                {
                    Format = "jpg",
                    Width = 1024,
                    Height = 768,
                    Quality = 90,

                };
                
                ImageBuilder.Current.Build(stream, ms, settings2, false);


                await pagePreviewCallback(ms, "0.jpg");
            }
        }


    }

    public class Gradient : BuilderExtension, IPlugin, IQuerystringPlugin
    {

       

        protected override RequestedAction PostRenderImage(ImageState s)
        {
            var bitmap = s.destBitmap;
            var radius = 50;
            var blurLevel = s.settings["r.blur"];
            if (blurLevel != null && int.TryParse(blurLevel, out radius))
            {

            }
            s.destBitmap =  FastBoxBlur(bitmap, radius);
            return RequestedAction.None;
        }

        public IEnumerable<string> GetSupportedQuerystringKeys()
        {
            return new string[] { "r.blur" };
        }

        public IPlugin Install(ImageResizer.Configuration.Config c)
        {
            c.Plugins.add_plugin(this);
            return this;
        }

        public bool Uninstall(ImageResizer.Configuration.Config c)
        {
            c.Plugins.remove_plugin(this);
            return true;
        }


      
        
        private Bitmap FastBoxBlur(Bitmap img, int radius)
        {
            int kSize = radius;
            if (kSize % 2 == 0) kSize++;
            Bitmap Hblur = (Bitmap)img.Clone();
            float Avg = (float)1 / kSize;

            for (int j = 0; j < img.Height; j++)
            {

                float[] hSum = new float[] { 0f, 0f, 0f, 0f };
                float[] iAvg = new float[] { 0f, 0f, 0f, 0f };

                for (int x = 0; x < kSize; x++)
                {
                    Color tmpColor = img.GetPixel(x, j);
                    hSum[0] += tmpColor.A;
                    hSum[1] += tmpColor.R;
                    hSum[2] += tmpColor.G;
                    hSum[3] += tmpColor.B;
                }
                iAvg[0] = hSum[0] * Avg;
                iAvg[1] = hSum[1] * Avg;
                iAvg[2] = hSum[2] * Avg;
                iAvg[3] = hSum[3] * Avg;
                for (int i = 0; i < img.Width; i++)
                {
                    if (i - kSize / 2 >= 0 && i + 1 + kSize / 2 < img.Width)
                    {
                        Color tmp_pColor = img.GetPixel(i - kSize / 2, j);
                        hSum[0] -= tmp_pColor.A;
                        hSum[1] -= tmp_pColor.R;
                        hSum[2] -= tmp_pColor.G;
                        hSum[3] -= tmp_pColor.B;
                        Color tmp_nColor = img.GetPixel(i + 1 + kSize / 2, j);
                        hSum[0] += tmp_nColor.A;
                        hSum[1] += tmp_nColor.R;
                        hSum[2] += tmp_nColor.G;
                        hSum[3] += tmp_nColor.B;
                        //
                        iAvg[0] = hSum[0] * Avg;
                        iAvg[1] = hSum[1] * Avg;
                        iAvg[2] = hSum[2] * Avg;
                        iAvg[3] = hSum[3] * Avg;
                    }

                    Hblur.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
                }
            }
            Bitmap total = (Bitmap)Hblur.Clone();
            for (int i = 0; i < Hblur.Width; i++)
            {
                float[] tSum = new float[] { 0f, 0f, 0f, 0f };
                float[] iAvg = new float[] { 0f, 0f, 0f, 0f };
                for (int y = 0; y < kSize; y++)
                {
                    Color tmpColor = Hblur.GetPixel(i, y);
                    tSum[0] += tmpColor.A;
                    tSum[1] += tmpColor.R;
                    tSum[2] += tmpColor.G;
                    tSum[3] += tmpColor.B;
                }
                iAvg[0] = tSum[0] * Avg;
                iAvg[1] = tSum[1] * Avg;
                iAvg[2] = tSum[2] * Avg;
                iAvg[3] = tSum[3] * Avg;

                for (int j = 0; j < Hblur.Height; j++)
                {
                    if (j - kSize / 2 >= 0 && j + 1 + kSize / 2 < Hblur.Height)
                    {
                        Color tmp_pColor = Hblur.GetPixel(i, j - kSize / 2);
                        tSum[0] -= tmp_pColor.A;
                        tSum[1] -= tmp_pColor.R;
                        tSum[2] -= tmp_pColor.G;
                        tSum[3] -= tmp_pColor.B;
                        Color tmp_nColor = Hblur.GetPixel(i, j + 1 + kSize / 2);
                        tSum[0] += tmp_nColor.A;
                        tSum[1] += tmp_nColor.R;
                        tSum[2] += tmp_nColor.G;
                        tSum[3] += tmp_nColor.B;
                        //
                        iAvg[0] = tSum[0] * Avg;
                        iAvg[1] = tSum[1] * Avg;
                        iAvg[2] = tSum[2] * Avg;
                        iAvg[3] = tSum[3] * Avg;
                    }
                    total.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
                }
            }
            return total;
        }


    }

    public class TTT
    {

    }

}
