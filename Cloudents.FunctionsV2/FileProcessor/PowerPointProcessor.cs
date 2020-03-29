using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.GhostScript;
using Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Api;
using Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.FunctionsV2.FileProcessor
{
    public class PowerPointProcessor : IFileProcessor
    {
        private readonly ConvertDocumentApi _convertDocumentApi;
        static PowerPointProcessor()
        {
            Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Client.Configuration.Default.AddApiKey("Apikey", "07af4ce1-40eb-4e97-84e0-c02b4974b190");
            Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Client.Configuration.Default.Timeout = 300000; //base on support
        }

        public PowerPointProcessor()
        {
            _convertDocumentApi = new ConvertDocumentApi();
        }


        public async Task ProcessFileAsync(long id, CloudBlockBlob blob, IBinder binder, ILogger log, CancellationToken token)
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), id.ToString());
            try
            {
                Directory.CreateDirectory(tempDirectory);

                await using var sr = await blob.OpenReadAsync();

                var directory = blob.Parent;
                var textBlob = directory.GetBlockBlobReference("text.txt");
                
                textBlob.Properties.ContentType = "text/plain";
                var text2 = await _convertDocumentApi.ConvertDocumentPptxToTxtAsync(sr);

                var text = text2.TextResult;
                text = StripUnwantedChars(text);

                await textBlob.UploadTextAsync(text ?? string.Empty);
                sr.Seek(0, SeekOrigin.Begin);
                var bytes = await _convertDocumentApi.ConvertDocumentAutodetectToPdfAsync(sr);

                var inputFileNamePath = Path.Combine(tempDirectory, "in.pdf");

                await File.WriteAllBytesAsync(inputFileNamePath, bytes, token);
                log.LogInformation($"location of file is {inputFileNamePath}");
                var outputPath = Path.Combine(tempDirectory, "output");
                Directory.CreateDirectory(outputPath);
                log.LogInformation($"location of output is {outputPath}");
                GhostscriptWrapper.GeneratePageThumbs(inputFileNamePath, Path.Combine(outputPath, "%d.jpg"), 1, 1000,
                    150, 150);

                var files = Directory.GetFiles(outputPath);
                textBlob.Metadata["PageCount"] = files.Length.ToString();
                await textBlob.SetMetadataAsync();

                var tasks = new List<Task>();
                foreach (var file in files)
                {
                    var fileName = int.Parse(Path.GetFileNameWithoutExtension(file));
                    var blobToUpload = directory.GetBlockBlobReference($"preview-{--fileName}.jpg");
                    blob.Properties.ContentType = "image/jpeg";
                    var t = blobToUpload.UploadFromFileAsync(file);
                    tasks.Add(t);
                    //File.Delete(file);
                }

                await Task.WhenAll(tasks);

            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
            //var text = text2.TextResult;
            //if (result.Successful == false)
            //{
            //    throw new ArgumentException($"ConvertDocumentAutodetectToPngArrayAsync return false in id {id}");
            //}

            //var imagesResult = result.PngResultPages;
            //var pageCount = imagesResult.Count;


            //textBlob.Metadata["PageCount"] = pageCount.ToString();
            //await textBlob.SetMetadataAsync();

            //var starter = await binder.BindAsync<IDurableOrchestrationClient>(new DurableClientAttribute(), token);


            //await starter.PurgeInstanceHistoryAsync($"ProcessPowerPoint-{id}");
            //await starter.StartNewAsync("ProcessPowerPoint", $"ProcessPowerPoint-{id}", new PowerPointOrchestrationInput
            //{
            //    Id = id,
            //    Images = imagesResult
            //});

            //var httpClient = await binder.BindAsync<HttpClient>(new HttpClientFactoryAttribute(), token);
            //var listOfTasks = imagesResult.Select(async imageResult =>
            //{
            //    var previewBlob = directory.GetBlockBlobReference($"preview-{imageResult.PageNumber - 1}.jpg");
            //    previewBlob.Properties.ContentType = "image/jpeg";
            //    await using var imageStream = await httpClient.GetStreamAsync(imageResult.URL);
            //    using var input = Image.Load<Rgba32>(imageStream);
            //    await using var blobWriteStream = await previewBlob.OpenWriteAsync();
            //    input.SaveAsJpeg(blobWriteStream);
            //});
            //await Task.WhenAll(listOfTasks);

        }


        private static string StripUnwantedChars(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            //input = Regex.Replace(input, "\\b(\\S)\\s+(?=\\S)", string.Empty);
            //Remove white spaces
            var result = Regex.Replace(input, @"\s+", " ");

            //Remove ids from document text
            var eightOrNineDigitsId = new Regex(@"\b\d{8,9}\b", RegexOptions.Compiled);
            result = eightOrNineDigitsId.Replace(result, string.Empty);
            var sb = new StringBuilder(new string(result.Where(w => char.IsLetterOrDigit(w) || char.IsWhiteSpace(w)).ToArray()));
            sb.Replace(
                "אזהרההנך רשאי להשתמש  שימוש הוגן  ביצירה מוגנת למטרות שונותלרבות  לימוד עצמי  ואין לעשות שימוש בעל אופי מסחרי או מעיןמסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצהשעמל על הכנת ההרצאות והחומר לציבור התלמידים",
                string.Empty);
            sb.Replace("בס\"ד", string.Empty);
            return sb.Replace("find more resources at oneclass.com", string.Empty).ToString();
        }
    }

    public class PowerPointOrchestrationInput
    {
        public long Id { get; set; }
        public List<ConvertedPngPage> Images { get; set; }
    }

    public class PowerPointActivityInput
    {
        public long Id { get; set; }
        public ConvertedPngPage Image { get; set; }
    }
}
