using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class DocumentController : ApiController
    {
        private readonly IBlobProvider2<FilesContainerName> m_BlobProviderFiles;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IGuidIdGenerator m_GuidGenerator;


        public DocumentController(IBlobProvider2<FilesContainerName> blobProviderFiles, IQueueProvider queueProvider, IZboxWriteService zboxWriteService, IGuidIdGenerator guidGenerator)
        {
            m_BlobProviderFiles = blobProviderFiles;
            m_QueueProvider = queueProvider;
            m_ZboxWriteService = zboxWriteService;
            m_GuidGenerator = guidGenerator;
        }

        // GET api/Document
        public async Task<string> Get(long itemId, string blob, CancellationToken cancellationToken)
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


        [Route("api/document/like")]
        [HttpPost]
        [Authorize]
        public async Task<HttpResponseMessage> Like(ItemLikeRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var id = m_GuidGenerator.GetId();
            var command = new RateItemCommand(model.Id, User.GetUserId(), id);
            await m_ZboxWriteService.RateItemAsync(command);
            return Request.CreateResponse();
        }

        [Route("api/document/tag")]
        [HttpPut]
        [Authorize]
        public HttpResponseMessage AddTag(ItemTagRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var z = new AssignTagsToDocumentCommand(model.ItemId, model.Tags, TagType.User);
            m_ZboxWriteService.AddItemTag(z);
            return Request.CreateResponse();
        }

    }
}
