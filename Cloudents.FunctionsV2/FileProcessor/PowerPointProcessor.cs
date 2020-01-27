﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.FunctionsV2.Binders;
using Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Api;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Cloudents.FunctionsV2.FileProcessor
{
    public class PowerPointProcessor : IFileProcessor
    {
        public async Task ProcessFileAsync(long id, CloudBlockBlob blob, IBinder binder, ILogger log, CancellationToken token)
        {
            Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Client.Configuration.Default.AddApiKey("Apikey", "86afd89a-207c-4e7a-9ffc-da23fcb9d5b7");
            Cloudmersive.APIClient.NETCore.DocumentAndDataConvert.Client.Configuration.Default.Timeout = 300000; //base on support

            var apiInstance = new ConvertDocumentApi();

            using (var sr = await blob.OpenReadAsync())
            {
                //var pageCount2 = await apiInstance.ConvertDocumentAutodetectGetInfoAsync(sr);
                //sr.Seek(0, SeekOrigin.Begin);
                var text2 = await apiInstance.ConvertDocumentPptxToTxtAsync(sr);
                sr.Seek(0, SeekOrigin.Begin);

                var result = await apiInstance.ConvertDocumentAutodetectToPngArrayAsync(sr);

                var directory = blob.Parent;


                var text = text2.TextResult;
                if (result.Successful == false)
                {
                    throw new ArgumentException($"ConvertDocumentAutodetectToPngArrayAsync return false in id {id}");
                }
                var imagesResult = result.PngResultPages;
                //var imageUrls = imagesResult.Select(s => s.URL);

                var pageCount = imagesResult.Count;

                var textBlob = directory.GetBlockBlobReference("text.txt");
                textBlob.Properties.ContentType = "text/plain";
                text = StripUnwantedChars(text);
                textBlob.Metadata["PageCount"] = pageCount.ToString();
                await textBlob.UploadTextAsync(text ?? string.Empty);


                var httpClient = await binder.BindAsync<HttpClient>(new HttpClientFactoryAttribute(), token);
                foreach (var imageResult in imagesResult)
                {
                    var previewBlob = directory.GetBlockBlobReference($"preview-{imageResult.PageNumber - 1}.jpg");
                    previewBlob.Properties.ContentType = "image/jpeg";
                    using (var imageStream = await httpClient.GetStreamAsync(imageResult.URL))
                    using (var input = Image.Load<Rgba32>(imageStream))
                    using (var blobWriteStream = await previewBlob.OpenWriteAsync())
                    {

                        input.SaveAsJpeg(blobWriteStream);
                        // ResizeImage(input, imageSmall, ImageSize.Small, format);
                    }
                    //{

                    //await previewBlob.UploadFromStreamAsync(imageStream);
                    //}
                }



                //apiInstance.ConvertDocumentAutodetectGetInfo()
                //var result = apiInstance.ConvertDocumentAutodetectToPngArray(inputFile);

                // Word DOCX to PDF
                //Object result = apiInstance.ConvertDocumentDocxToPdf(inputFile);


            }
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
}