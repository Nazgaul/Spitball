using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Image = SixLabors.ImageSharp.Image;
using Size = SixLabors.Primitives.Size;

namespace Cloudents.FunctionsV2.Services
{
    //public class TextToImage : ITextToImage
    //{
    //    static TextToImage()
    //    {
    //        Cloudmersive.APIClient.NETCore.ImageRecognition.Client.Configuration.Default.AddApiKey("Apikey", "07af4ce1-40eb-4e97-84e0-c02b4974b190");
    //    }


    //    private readonly EditApi _apiInstance = new EditApi();

    //    public async Task<Image> ConvertAsync(string text, Size rectangle)
    //    {
    //        var image = new Image<Rgba32>(rectangle.Width, rectangle.Height);
    //        image.Mutate(c => c.BackgroundColor(Color.Aqua));
    //        var ms = new MemoryStream();

    //        image.SaveAsPng(ms);
    //        var bytes = ms.ToArray();

    //        var result = await _apiInstance.EditDrawTextAsync(
    //            new DrawTextRequest(
    //                BaseImageBytes: bytes,
    //                TextToDraw: new List<DrawTextInstance>()
    //                {
    //                    new DrawTextInstance(
    //                        text,
    //                        FontFamilyName: "Georgia",
    //                        FontSize:32,
    //                        Color:"black",0,0,rectangle.Width,rectangle.Height
    //                    )
    //                }));

    //        return Image.LoadPixelData<Rgba32>(result, rectangle.Width, rectangle.Height);
    //    }

    //}

    public class TextToImageGdi : ITextToImage
    {
        public Image Convert(string text, SixLabors.Fonts.Font font, string hexRgb, Size rectangle)
        {
            using var myBitmap = new Bitmap(rectangle.Width, rectangle.Height);
            
            using (Graphics g = Graphics.FromImage(myBitmap))
            {
                var color = System.Drawing.ColorTranslator.FromHtml(hexRgb);
                var brush = new System.Drawing.SolidBrush(color);
                //var text = "בקרוב תראו תוצאות וציונים שיעלו לכם חיוך על הפנים";
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g.DrawString(text,
                    new Font(font.Name, font.Size),
                    brush,
                    new System.Drawing.RectangleF(0, 0, myBitmap.Width, myBitmap.Height),
                    new StringFormat()
                    {
                        Alignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisWord

                    });
                //g.DrawString("My Text very very nice",
                //    new Font("Tahoma", 20),
                //    Brushes.White,
                //    new PointF(0, 0));
            }

            using var ms = new MemoryStream();
            myBitmap.Save(ms, ImageFormat.Bmp);
            var image =  Image.Load(ms.ToArray());
            for (int i = myBitmap.Width - 1; i >= 0; i--)
            {
                for (int j = myBitmap.Height - 1; j >= 0; j--)
                {
                    
                }
               
                
            }

            return image;



        }
    }

    public interface ITextToImage
    {
        Image Convert(string text, SixLabors.Fonts.Font font, string hexRgb, Size rectangle);
    }
}
