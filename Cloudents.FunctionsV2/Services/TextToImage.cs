//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.Drawing.Text;
//using System.IO;
//using Image = SixLabors.ImageSharp.Image;
//using Size = SixLabors.Primitives.Size;

//namespace Cloudents.FunctionsV2.Services
//{
//    public class TextToImageGdi : ITextToImage
//    {
//        public Image Convert(string text,  int size, string hexRgb, Size rectangle)
//        {
//            using var myBitmap = new Bitmap(rectangle.Width, rectangle.Height + 10);

//            using (Graphics g = Graphics.FromImage(myBitmap))
//            {
//                g.SmoothingMode = SmoothingMode.AntiAlias;
//                g.TextRenderingHint = TextRenderingHint.AntiAlias;
//                var color = ColorTranslator.FromHtml(hexRgb);
//                var brush = new SolidBrush(color);
//                g.DrawString(text,
//                    new Font("Calibri", size, FontStyle.Regular, GraphicsUnit.Pixel),
//                    brush,
//                    new RectangleF(0, 0, rectangle.Width, rectangle.Height),
//                    new StringFormat()
//                    {
//                        Alignment = StringAlignment.Center,
//                        Trimming = StringTrimming.EllipsisWord,

//                    });
               
//            }

//            using var ms = new MemoryStream();
//            myBitmap.Save(ms, ImageFormat.Png);
//            var image = Image.Load(ms.ToArray());
//            return image;



//        }
//    }

//    //public interface ITextToImage
//    //{
//    //    Image Convert(string text,  int size, string hexRgb, Size rectangle);
//    //}
//}
