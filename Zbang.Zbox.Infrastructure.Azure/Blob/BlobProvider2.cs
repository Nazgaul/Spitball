using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Infrastructure.Azure.Blob
{
    public class BlobProvider2<T> : BlobProvider, IBlobProvider2<T> where T : IStorageContainerName, new()
    {
        //private CloudBlobClient m_BlobClient;
        // private readonly ILocalStorageProvider m_LocalStorageProvider;
        private readonly T m_Container = new T();
       
        public BlobProvider2(ILocalStorageProvider localStorageProvider)
            : base(localStorageProvider)
        {
        }

        private CloudBlockBlob GetBlob(string blobName)
        {
            var container = BlobClient.GetContainerReference(m_Container.Name.ToLower());
            var directory = container.GetDirectoryReference(m_Container.RelativePath);
            return directory.GetBlockBlobReference(blobName);
        }

        public async Task<int> UploadFileBlockAsync(string blobName, Stream fileContent, int currentIndex)
        {

            var blob = GetBlob(blobName);
            fileContent.Seek(0, SeekOrigin.Begin);
            await blob.PutBlockAsync(ToBase64(currentIndex), fileContent, null);
            return ++currentIndex;
        }



        public async Task CommitBlockListAsync(string blobName, int currentIndex, string contentType)
        {
            var blockList = Enumerable.Range(0, currentIndex).Select(ToBase64);
            var blob = GetBlob(blobName);
            blob.Properties.ContentType = contentType;
            blob.Properties.CacheControl = "private max-age=" + TimeConst.Week;
            await blob.PutBlockListAsync(blockList);
           
        }

        public Task UploadStreamAsync(string blobName, Stream content, string mimeType, CancellationToken token)
        {
            content.Seek(0, SeekOrigin.Begin);
            var blob = GetBlob(blobName);
            //var blob = BlobClient.GetContainerReference(AzurePreviewContainer).GetBlockBlobReference(blobName);
            blob.Properties.ContentType = mimeType;
            blob.Properties.CacheControl = "public, max-age=" + TimeConst.Year;
            return blob.UploadFromStreamAsync(content, token);
        }

        public string GetBlobUrl(string blobName)
        {
            return GetBlob(blobName).Uri.AbsoluteUri;
        }

        public Task<bool> ExistsAsync(string blobName)
        {
            var blob = GetBlob(blobName);
            return blob.ExistsAsync();
        }
        public bool Exists(string blobName)
        {
            var blob = GetBlob(blobName);
            return blob.Exists();
        }
    }
}