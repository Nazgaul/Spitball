using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GhostscriptSharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Cloudents.FunctionsV2
{
    public static class GhostscriptFunction
    {
        [FunctionName("GhostscriptFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/ghostscript/{id}")] HttpRequest req,
            string id,
            [Blob("spitball-files/files/{id}")]CloudBlobDirectory directory,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var segment = await directory.ListBlobsSegmentedAsync(null);
            var originalBlob = (CloudBlockBlob)segment.Results.FirstOrDefault(f2 => f2.Uri.Segments.Last().StartsWith("file-"));
            if (originalBlob == null)
            {
                return new OkObjectResult("No file");
            }

            var inputFile = Path.Combine(Path.GetTempPath(), "in.pdf");
            await originalBlob.DownloadToFileAsync(inputFile, FileMode.Create);
            //Path.Combine(Path.GetTempPath(), "out%d.jpg");

            var z = Path.Combine(Path.GetTempPath(), id);
            var outputPath = Path.Combine(z, "%d.jpg");
            Directory.CreateDirectory(z);
            GhostscriptWrapper.GeneratePageThumbs(inputFile, outputPath, 1, 1000, 300, 300);

            foreach (var file in Directory.GetFiles(z))
            {
                var fileName = int.Parse(Path.GetFileNameWithoutExtension(file));
                var blobToUpload = directory.GetBlockBlobReference($"preview-{--fileName}.jpg");
                await blobToUpload.UploadFromFileAsync(file);
                File.Delete(file);
            }
            Directory.Delete(z);
            return new OkObjectResult("Done");

            //foreach (var VARIABLE in COLLECTION)
            //{

            //}

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //return name != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
