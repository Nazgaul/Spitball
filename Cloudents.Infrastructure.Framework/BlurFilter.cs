//using System.Collections.Generic;
//using System.Drawing;
//using ImageResizer.Plugins;
//using ImageResizer.Resizing;

//namespace Cloudents.Infrastructure.Framework
//{
//    public class BlurFilter : BuilderExtension, IPlugin, IQuerystringPlugin
//    {
//        protected override RequestedAction PostRenderImage(ImageState s)
//        {
//            var bitmap = s.destBitmap;
//            if (s.settings["r.blur"] == null)
//            {
//                return RequestedAction.None;
//            }
//            var blurLevel = int.Parse(s.settings["r.blur"] ?? 10.ToString());
//            var startOfPage = bool.Parse(s.settings["r.blurStart"] ?? bool.FalseString);


//            if (startOfPage)
//            {
//                using (var imageToProcess = bitmap.Clone(
//                    new Rectangle(0, bitmap.Height / 2, bitmap.Width, bitmap.Height / 2),
//                    bitmap.PixelFormat))
//                {
//                    using (var e = Graphics.FromImage(bitmap))
//                    {

//                        var result = FastBoxBlur(imageToProcess, blurLevel);
//                        e.DrawImage(result, new Point(0, bitmap.Height / 2));
//                        s.destBitmap = bitmap;
//                        return RequestedAction.None;
//                    }
//                }
//            }

//            s.destBitmap = FastBoxBlur(bitmap, blurLevel);
//            return RequestedAction.None;
//        }

//        public IEnumerable<string> GetSupportedQuerystringKeys()
//        {
//            return new[] { "r.blur", "r.blurStart" };
//        }

//        public IPlugin Install(ImageResizer.Configuration.Config c)
//        {
//            c.Plugins.add_plugin(this);
//            return this;
//        }

//        public bool Uninstall(ImageResizer.Configuration.Config c)
//        {
//            c.Plugins.remove_plugin(this);
//            return true;
//        }




//        private Bitmap FastBoxBlur(Bitmap img, int radius)
//        {
//            //int GetStartOfHeight()
//            //{
//            //    if (partPage)
//            //    {
//            //        return img.Height / 2;
//            //    }

//            //    return 0;

//            //}

//            int kSize = radius;
//            if (kSize % 2 == 0) kSize++;
           

//            // var e = System.Drawing.Graphics.FromImage(img);
//            Bitmap hblur = (Bitmap)img.Clone();
//            float avg = (float)1 / kSize;


//            for (int j = 0; j < img.Height; j++)
//            {

//                float[] hSum = new float[] { 0f, 0f, 0f, 0f };
//                float[] iAvg = new float[] { 0f, 0f, 0f, 0f };

//                for (int x = 0; x < kSize; x++)
//                {
//                    Color tmpColor = img.GetPixel(x, j);
//                    hSum[0] += tmpColor.A;
//                    hSum[1] += tmpColor.R;
//                    hSum[2] += tmpColor.G;
//                    hSum[3] += tmpColor.B;
//                }
//                iAvg[0] = hSum[0] * avg;
//                iAvg[1] = hSum[1] * avg;
//                iAvg[2] = hSum[2] * avg;
//                iAvg[3] = hSum[3] * avg;
//                for (int i = 0; i < img.Width; i++)
//                {
//                    if (i - kSize / 2 >= 0 && i + 1 + kSize / 2 < img.Width)
//                    {
//                        Color tmpPColor = img.GetPixel(i - kSize / 2, j);
//                        hSum[0] -= tmpPColor.A;
//                        hSum[1] -= tmpPColor.R;
//                        hSum[2] -= tmpPColor.G;
//                        hSum[3] -= tmpPColor.B;
//                        Color tmpNColor = img.GetPixel(i + 1 + kSize / 2, j);
//                        hSum[0] += tmpNColor.A;
//                        hSum[1] += tmpNColor.R;
//                        hSum[2] += tmpNColor.G;
//                        hSum[3] += tmpNColor.B;
//                        //
//                        iAvg[0] = hSum[0] * avg;
//                        iAvg[1] = hSum[1] * avg;
//                        iAvg[2] = hSum[2] * avg;
//                        iAvg[3] = hSum[3] * avg;
//                    }

//                    hblur.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
//                }
//            }
//            Bitmap total = (Bitmap)hblur.Clone();
//            for (int i = 0; i < hblur.Width; i++)
//            {
//                float[] tSum = new float[] { 0f, 0f, 0f, 0f };
//                float[] iAvg = new float[] { 0f, 0f, 0f, 0f };
//                for (int y = 0; y < kSize; y++)
//                {
//                    Color tmpColor = hblur.GetPixel(i, y);
//                    tSum[0] += tmpColor.A;
//                    tSum[1] += tmpColor.R;
//                    tSum[2] += tmpColor.G;
//                    tSum[3] += tmpColor.B;
//                }
//                iAvg[0] = tSum[0] * avg;
//                iAvg[1] = tSum[1] * avg;
//                iAvg[2] = tSum[2] * avg;
//                iAvg[3] = tSum[3] * avg;

//                for (int j = 0; j < hblur.Height; j++)
//                {
//                    if (j - kSize / 2 >= 0 && j + 1 + kSize / 2 < hblur.Height)
//                    {
//                        Color tmpPColor = hblur.GetPixel(i, j - kSize / 2);
//                        tSum[0] -= tmpPColor.A;
//                        tSum[1] -= tmpPColor.R;
//                        tSum[2] -= tmpPColor.G;
//                        tSum[3] -= tmpPColor.B;
//                        Color tmpNColor = hblur.GetPixel(i, j + 1 + kSize / 2);
//                        tSum[0] += tmpNColor.A;
//                        tSum[1] += tmpNColor.R;
//                        tSum[2] += tmpNColor.G;
//                        tSum[3] += tmpNColor.B;
//                        //
//                        iAvg[0] = tSum[0] * avg;
//                        iAvg[1] = tSum[1] * avg;
//                        iAvg[2] = tSum[2] * avg;
//                        iAvg[3] = tSum[3] * avg;
//                    }
//                    total.SetPixel(i, j, Color.FromArgb((int)iAvg[0], (int)iAvg[1], (int)iAvg[2], (int)iAvg[3]));
//                }
//            }

//            //if (partPage)
//            //{
//            //    e.DrawImage(total, new Point(0, img.Height / 2));
//            //}
//            //else
//            //{

//            //}

//            return total;
//        }


//    }
//}