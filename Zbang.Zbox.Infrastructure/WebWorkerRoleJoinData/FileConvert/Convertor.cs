
using Zbang.Zbox.Infrastructure.Storage;
using System.IO;
using System.Drawing;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert
{
    public abstract class Convertor
    {
        protected IBlobProvider m_BlobProvider;
        protected string m_BlobName;

        protected Convertor(IBlobProvider blobProvider, string blob)
        {
            m_BlobProvider = blobProvider;
            m_BlobName = blob;
        }

        protected Stream ReadBlob(bool atOnce = false)
        {
            if (!m_BlobProvider.CheckIfFileExists(m_BlobName))
            {
                throw new ItemNotFoundException("Blob does not exists");
            }
            if (atOnce)
            {
                return m_BlobProvider.DownloadFile(m_BlobName);
            }
            return m_BlobProvider.DownloadFile(m_BlobName, null);// blob.OpenRead();
        }


        public abstract void ConvertFileToWebSitePreview();
        public abstract Image ConvertFileToImage();

        protected void SaveToBlob(Stream stream, string mimeType = "application/pdf")
        {
            m_BlobProvider.UploadFileToCacheAsync(m_BlobName, stream, mimeType);
            //var cacheblob = m_BlobProvider.ZboxCacheContainerFile(m_BlobName);
            //stream.Position = 0;
            //cacheblob.UploadFromStream(stream);
            //cacheblob.Properties.ContentType = "application/pdf";
            //cacheblob.Properties.CacheControl = "private, max-age=" + TimeConsts.Minute * 10;
            //cacheblob.SetProperties();
            //cacheblob.Metadata.Add(JoinConst.LastAccessTimeMetaDataKey, DateTime.UtcNow.ToString());
            //cacheblob.SetMetadata();
        }
    }
}
