using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class DocumentController : ApiController
    {
        private readonly IBlobProvider2<FilesContainerName> m_BlobProviderFiles;

        public DocumentController(IBlobProvider2<FilesContainerName> blobProviderFiles)
        {
            m_BlobProviderFiles = blobProviderFiles;
        }

        // GET api/Document
        public string Get(string blob)
        {
            if (string.IsNullOrEmpty(blob))
            {
                return "hello world";
            }
            return m_BlobProviderFiles.GenerateSharedAccessReadPermission(blob, 20);
        }


        
    }
}
