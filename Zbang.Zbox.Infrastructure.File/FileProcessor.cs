using System;
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
        protected const int NumberOfFilesInGroup = 20;
        protected const int ThumbnailWidth = 148;
        protected const int ThumbnailHeight = 187;


        protected readonly IBlobProvider BlobProvider;

        protected FileProcessor(IBlobProvider blobProvider)
        {
            BlobProvider = blobProvider;
        }

        protected int CalculateTillWhenToDrawPictures(int indexNum)
        {
            var indexOfPageGenerate = NumberOfFilesInGroup;
            if (indexNum > NumberOfFilesInGroup / 2)
            {
                indexOfPageGenerate = (indexNum / 10) * 10 + NumberOfFilesInGroup;
            }
            return indexOfPageGenerate;
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

        protected async Task UploadMetaData(string fileContent, string blobName)
        {
            if (string.IsNullOrEmpty(fileContent))
            {
                return;
            }
            var metaData =await BlobProvider.FetechBlobMetaDataAsync(blobName);
            if (metaData == null)
            {
                metaData = new Dictionary<string, string>();
            }
            metaData[StorageConsts.ContentMetaDataKey] = fileContent;
            await BlobProvider.SaveMetaDataToBlobAsync(blobName, metaData);
        }

        //protected async Task UploadPreviewToAzure(string blobName, int startPage, int stopPage)
        //{
        //    var meta = await BlobProvider.FetechBlobMetaDataAsync(blobName);

        //}
    }
}
