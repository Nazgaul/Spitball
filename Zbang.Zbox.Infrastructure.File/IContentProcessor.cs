using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.File
{
    public interface IContentProcessor
    {
        //byte[] ConvertFileToWebSitePreview(Stream stream, int width, int height);
        Task<PreviewResult> ConvertFileToWebSitePreview(Uri contentUrl, int width, int height, int indexNum);

        //string TypeOfView { get; }

        bool CanProcessFile(Uri contentUrl);

        //TODO: split to a different interface
        Task<PreProcessFileResult> PreProcessFile(Uri blobUri);

        string GetDefaultThumbnailPicture();
     
    }

   
}
