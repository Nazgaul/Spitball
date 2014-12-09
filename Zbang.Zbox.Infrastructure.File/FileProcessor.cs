using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public abstract class FileProcessor : ContentProcessor , IContentProcessor
    {
        protected const int NumberOfFilesInGroup = 20;
        protected const int ThumbnailWidth = 148;
        protected const int ThumbnailHeight = 187;

        protected readonly IBlobProvider BlobProvider;

        protected FileProcessor(IBlobProvider blobProvider)
        {
            BlobProvider = blobProvider;
        }
       
        protected  int CalculateTillWhenToDrawPictures(int indexNum)
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
    }
}
