using Microsoft.WindowsAzure.StorageClient;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ViewModel.DTOs;

namespace Zbang.Zbox.Mvc3WebRole.Helpers
{
    public class DownloadBoxItem 
    {
         const string DefaultMimeType = "application/octet-stream";

        readonly FileDto m_FileData;
        readonly IBlobProvider m_BlobProvider;

         public DownloadBoxItem(FileDto fileData, IBlobProvider blobProvider)
           
        {
            m_FileData = fileData;
            m_BlobProvider = blobProvider;
        }

        public System.Web.Mvc.ActionResult DownloadItem()
        {
            var blob = m_BlobProvider.GetFile(m_FileData.BlobName);
            if (!blob.Exists())
            {
                throw new StorageClientException();
            }

            var contentType = DefaultMimeType;
            if (!string.IsNullOrWhiteSpace(blob.Properties.ContentType))
            {
                contentType = blob.Properties.ContentType;
            }
            return new BlobFileStream(blob, contentType, m_FileData.Name, true);


        }
    }
}