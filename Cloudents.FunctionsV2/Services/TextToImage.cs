using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cloudmersive.APIClient.NETCore.ImageRecognition.Api;
using Cloudmersive.APIClient.NETCore.ImageRecognition.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Cloudents.FunctionsV2.Services
{
    public class TextToImage : ITextToImage
    {
        static TextToImage()
        {
            Cloudmersive.APIClient.NETCore.ImageRecognition.Client.Configuration.Default.AddApiKey("Apikey", "07af4ce1-40eb-4e97-84e0-c02b4974b190");
        }


        private readonly EditApi _apiInstance = new EditApi();

        public async Task<Image> ConvertAsync(string text, Size rectangle)
        {
            var image = new Image<Rgba32>(rectangle.Width, rectangle.Height);
            image.Mutate(c => c.BackgroundColor(Color.Aqua));
            var ms = new MemoryStream();
            
            image.SaveAsPng(ms);
            var bytes = ms.ToArray();

            var result = await _apiInstance.EditDrawTextAsync(
                new DrawTextRequest(
                    BaseImageBytes: bytes,
                    TextToDraw: new List<DrawTextInstance>()
                    {
                        new DrawTextInstance(
                            text,
                            FontFamilyName: "Georgia",
                            FontSize:32,
                            Color:"black",0,0,rectangle.Width,rectangle.Height
                        )
                    }));

            return Image.LoadPixelData<Rgba32>(result, rectangle.Width, rectangle.Height);
        }

    }

    public interface ITextToImage
    {
        Task<Image> ConvertAsync(string text, Size rectangle);
    }
}
