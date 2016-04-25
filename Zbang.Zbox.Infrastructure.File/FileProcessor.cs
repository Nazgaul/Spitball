﻿using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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


        public abstract Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken));

        public abstract Task<string> ExtractContentAsync(Uri blobUri,
            CancellationToken cancelToken = default(CancellationToken));


     

        //public virtual string GetDefaultThumbnailPicture()
        //{
        //    return DefaultPicture.DefaultFileTypePicture;
        //}

        protected string StripUnwantedChars(string input)
        {
            var spaceReg = new Regex(@"\s+", RegexOptions.Compiled);
            var eightOrNineDigitsId = new Regex(@"\b\d{8,9}\b", RegexOptions.Compiled);
            input = spaceReg.Replace(input, " ");
            input = eightOrNineDigitsId.Replace(input, string.Empty);
            //input = input.Replace("‏אזהרה‏ הנך רשאי להשתמש ' שימוש הוגן ' ביצירה מוגנת למטרות שונות, לרבות ' לימוד עצמי ' ואין לעשות שימוש בעל אופי מסחרי או מעין-מסחרי בסיכומי הרצאות תוך פגיעה בזכות היוצר של המרצה, שעמל על הכנת ההרצאות והחומר לציבור התלמידים.", string.Empty);
            input = input.Replace("\0", string.Empty);
            input = input.Replace("בס\"ד", string.Empty);
            return input;
        }

        protected async Task UploadMetaDataAsync(
            string blobName,
            int pageCount,
            string getCacheVersionPrefix
            )
        {


            var metaData = await BlobProvider.FetechBlobMetaDataAsync(blobName) ?? new Dictionary<string, string>();
            metaData = RemoveOldMetaTags(metaData, getCacheVersionPrefix);
            metaData[PagesInDocsMetaKey] = pageCount.ToString(CultureInfo.InvariantCulture);
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

        
    }
}
