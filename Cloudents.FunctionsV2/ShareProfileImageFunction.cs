using System;
using System.IO;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Di;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Cloudents.FunctionsV2
{
    public static class ShareProfileImageFunction
    {
        [FunctionName("ShareProfileImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/profile")] HttpRequest req,
            [Blob("spitball/placeholder/Profile-Background.jpg", FileAccess.Read)] [NotNull]
            Stream backgroundStream,
            ILogger log)
        {
            if (backgroundStream == null) throw new ArgumentNullException(nameof(backgroundStream));
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];


            var image = Image.Load<Rgba32>(backgroundStream);
            return new ImageResult(image, TimeSpan.Zero);
        }
    }
}
