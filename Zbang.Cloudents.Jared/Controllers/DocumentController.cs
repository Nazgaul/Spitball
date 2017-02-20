using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class DocumentController : ApiController
    {
        private readonly IBlobProvider2<FilesContainerName> m_BlobProviderFiles;
        private readonly IQueueProvider m_QueueProvider;

        public DocumentController(IBlobProvider2<FilesContainerName> blobProviderFiles, IQueueProvider queueProvider)
        {
            m_BlobProviderFiles = blobProviderFiles;
            m_QueueProvider = queueProvider;
        }

        // GET api/Document
        public async Task<string> Get(long itemId,string blob,CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(blob))
            {
                return "hello world";
            }
            await m_QueueProvider.InsertMessageToTranactionAsync(
                    new StatisticsData4(
                      new StatisticsData4.StatisticItemData
                      {
                          Id = itemId,
                          Action = (int)StatisticsAction.View
                      }
                  , -1), cancellationToken);
            return m_BlobProviderFiles.GenerateSharedAccessReadPermission(blob, 20);
        }


        
    }
}
