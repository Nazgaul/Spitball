using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public abstract class FileProcessor : ContentProcessor, IContentProcessor
    {

        protected const int ThumbnailWidth = 148;
        protected const int ThumbnailHeight = 187;

        protected const string CacheVersionPrefix = "V";
        protected const string PagesInDocsMetaKey = "pageCount";


        protected readonly IBlobProvider BlobProvider;

        protected FileProcessor(IBlobProvider blobProvider)
        {
            BlobProvider = blobProvider;
        }



        protected string GetBlobNameFromUri(Uri blobUri)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            return blobName;
        }


        public abstract Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken));


        public abstract string GetDefaultThumbnailPicture();

        protected string StripUnwantedChars(string input)
        {
            input = Regex.Replace(input, @"\s+", " ");
            input = input.Replace("‏אזהרה‏ הנך רשאי להשתמש ' שימוש הוגן ' ביצירה מוגנת למטרות שונות, לרבות ' לימוד עצמי ' ואין לעשות שימוש בעל אופי מסחרי או מעין-מסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצה, שעמל על הכנת ההרצאות והחומר לציבור התלמידים.", string.Empty);
            input = input.Replace("\0", string.Empty);
            return input;
        }

        protected async Task UploadMetaData(string fileContent,
            string blobName,
            int pageCount,
            string getCacheVersionPrefix)
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                return;
            }
            const int allowedChars = 8000;
            const int sizeOfMetaPerPage = 20;
            const int sizeOfPageCount = 10;


            var metaData = await BlobProvider.FetechBlobMetaDataAsync(blobName);
            if (metaData == null)
            {
                metaData = new Dictionary<string, string>();
            }
            metaData = RemoveOldMetaTags(metaData, getCacheVersionPrefix);
            metaData[PagesInDocsMetaKey] = pageCount.ToString(CultureInfo.InvariantCulture);
            var sizeToStrip = allowedChars - (sizeOfMetaPerPage * Math.Min(pageCount, 15) + sizeOfPageCount);
            metaData[StorageConsts.ContentMetaDataKey] = System.Net.WebUtility.UrlEncode(System.Net.WebUtility.HtmlEncode(fileContent.ToLower())).RemoveEndOfString(sizeToStrip);
            await BlobProvider.SaveMetaDataToBlobAsync(blobName, metaData);
        }

        protected IDictionary<string, string> RemoveOldMetaTags(IDictionary<string, string> metaTags, string cacheVersionPrfix)
        {


            var oldElements = metaTags.Where(w =>
                Regex.IsMatch(w.Key, @"\d") && !w.Key.StartsWith(cacheVersionPrfix)).Select(s => s.Key).ToList();



            foreach (var oldElement in oldElements)
            {
                metaTags.Remove(oldElement);
            }

            var z = metaTags.Select(s => s.Key).Where(w => w.StartsWith(cacheVersionPrfix)).Select(s =>
            {
                var number = s.Remove(0, cacheVersionPrfix.Length);
                return Convert.ToInt32(number);
            }).Where(p => p >= 15).ToList();

            foreach (int i in z)
            {
                metaTags.Remove(cacheVersionPrfix + i);
            }


            return metaTags;
        }

        //protected async Task UploadPreviewToAzure(string blobName, int startPage, int stopPage)
        //{
        //    var meta = await BlobProvider.FetechBlobMetaDataAsync(blobName);

        //}
    }
}
