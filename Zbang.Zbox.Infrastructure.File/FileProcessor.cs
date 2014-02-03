using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public abstract class FileProcessor : ContentProcessor , IContentProcessor
    {
        protected const int numberOfFilesInGroup = 20;
        protected const int ThumbnailWidth = 148;
        protected const int ThumbnailHeight = 187;

        protected readonly IBlobProvider m_BlobProvider;
        public FileProcessor(IBlobProvider blobProvider)
        {
            m_BlobProvider = blobProvider;
        }
       
        protected  int CalculateTillWhenToDrawPictures(int indexNum)
        {
            var indexOfPageGenerate = numberOfFilesInGroup;
            if (indexNum > numberOfFilesInGroup / 2)
            {
                indexOfPageGenerate = (indexNum / 10) * 10 + numberOfFilesInGroup;
            }
            return indexOfPageGenerate;
        }

        protected string GetBlobNameFromUri(Uri blobUri)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            return blobName;
        }


        public abstract Task<PreProcessFileResult> PreProcessFile(Uri blobUri);


        public abstract string GetDefaultThumbnailPicture();
    }
}
