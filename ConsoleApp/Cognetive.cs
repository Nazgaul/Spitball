using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace ConsoleApp
{
    public class Cognetive
    {
        public static IFaceClient Authenticate()
        {
            return new FaceClient(
                    new ApiKeyServiceClientCredentials("89af392451dc4be587fbfa6273a3d65a"))
            {
                Endpoint = "https://sb-cognitive-face.cognitiveservices.azure.com",
                
            };
        }


        /* 
 * DETECT FACES
 * Detects features from faces and IDs them.
 */
        public static async Task<FaceRectangle> DetectFaceExtract(IFaceClient client, string url)
        {
            Console.WriteLine("========DETECT FACES========");
            Console.WriteLine();

            
            // Create a list of images
            //List<string> imageFileNames = new List<string>
            //{
            //    "detection1.jpg", // single female with glasses
            //    // "detection2.jpg", // (optional: single man)
            //    // "detection3.jpg", // (optional: single male construction worker)
            //    // "detection4.jpg", // (optional: 3 people at cafe, 1 is blurred)
            //    "detection5.jpg", // family, woman child man
            //    "detection6.jpg" // elderly couple, male female
            //};
            //var url = "https://spitballdev.blob.core.windows.net/spitball-user/profile/160336/1564905368.PNG";
            var result = await client.Face.DetectWithUrlAsync(url);

            return result.FirstOrDefault()?.FaceRectangle;
            //foreach (var imageFileName in imageFileNames)
            //{
            //    IList<DetectedFace> detectedFaces;

            //    // Detect faces with all attributes from image url.
            //    detectedFaces = await client.Face.DetectWithUrlAsync($"{url}{imageFileName}",
            //        returnFaceAttributes: new List<FaceAttributeType>
            //        {
            //            FaceAttributeType.Accessories, FaceAttributeType.Age,
            //            FaceAttributeType.Blur, FaceAttributeType.Emotion, FaceAttributeType.Exposure,
            //            FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair,
            //            FaceAttributeType.HeadPose,
            //            FaceAttributeType.Makeup, FaceAttributeType.Noise, FaceAttributeType.Occlusion,
            //            FaceAttributeType.Smile
            //        },
            //        recognitionModel: recognitionModel);

            //    Console.WriteLine($"{detectedFaces.Count} face(s) detected from image `{imageFileName}`.");
            //}
        }
    }
}
