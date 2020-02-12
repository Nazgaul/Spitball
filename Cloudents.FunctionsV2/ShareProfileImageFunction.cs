using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.FunctionsV2.Binders;
using Cloudents.FunctionsV2.Di;
using Cloudents.FunctionsV2.Services;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class ShareProfileImageFunction
    {
        [FunctionName("ShareProfileImageFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/profile/{id:long}")] HttpRequest req, long id,
            [Blob("spitball/share-placeholder")] IEnumerable<CloudBlockBlob> directoryBlobs,
            [HttpClientFactory] HttpClient client,
            IBinder binder,
            [Inject] IQueryBus queryBus,
            [Inject] IHostUriService hostUriService,
            ILogger log,
            CancellationToken token)
        {
            //if (backgroundStream == null) throw new ArgumentNullException(nameof(backgroundStream));
            log.LogInformation("C# HTTP trigger function processed a request.");

            var query = new ShareProfileImageQuery(id);
            var result = await queryBus.QueryAsync(query, token);



            var uriBuilder = new UriBuilder(hostUriService.GetHostUri())
            {
                Path = "api/" + UrlConst.ImageFunctionUserRoute.Inject(new
                {
                    id,
                    file = result.Image
                }),
            }.AddQuery(new
            {
                width = 245,
                height = 245
            });

            await using var profileImageStream = await client.GetStreamAsync(uriBuilder.Uri);
            //foreach (var cloudBlockBlob in directoryBlobs)
            //{

            //}
            //var v = directoryBlobs.ToList();

            var bgBlob = $"share-placeholder/bg-profile-{(result.Country.MainLanguage.Info.TextInfo.IsRightToLeft ? "rtl" : "ltr")}.jpg";
            var blob = directoryBlobs.Single(s => s.Name == bgBlob);

            await using var bgBlobStream = await blob.OpenReadAsync();
            var image = Image.Load<Rgba32>(bgBlobStream);

            using var profileImage = Image.Load<Rgba32>(profileImageStream);
           

            image.Mutate(x=>x.DrawImage(profileImage, new Point(148,135),GraphicsOptions.Default));
            //using (var logoImage = Image.Load<Rgba32>(logoStream))
            //{


            //    image.Mutate(x => x.DrawImage(logoImage, new Point(139, 39), GraphicsOptions.Default));
            //}
            //if (result.Country == Country.Israel.Name)
            //{
            //    image.Mutate(x => x.Flip(FlipMode.Horizontal));
            //}

            //image.Mutate(x=>x.DrawImage());
            return new ImageResult(image, TimeSpan.Zero);

        }

        //public class ImageProperties
        //{
        //    public ImageProperties(string backgroundImage)
        //    {
        //        BackgroundImage = backgroundImage;
        //    }

        //    public string BackgroundImage { get; set; }
        //}

        //public Dictionary<Country, ImageProperties> ImageDictionary = new Dictionary<Country, ImageProperties>()
        //{
        //    [Country.Israel] = new ImageProperties("),
        //    [Country.UnitedStates] = new ImageProperties()
        //};




    }
}
