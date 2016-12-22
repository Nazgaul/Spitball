using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class FlashcardController : ApiController
    {
        private readonly IQueueProvider m_QueueProvider;
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IDocumentDbReadService m_DocumentDbReadService;
        //private readonly IZboxReadSecurityReadService m_ZboxReadSecurityService;
        //private readonly IZboxWriteService m_ZboxWriteService;

        public FlashcardController(IQueueProvider queueProvider, IZboxCacheReadService zboxReadService, IDocumentDbReadService documentDbReadService)
        {
            m_QueueProvider = queueProvider;
            m_ZboxReadService = zboxReadService;
            m_DocumentDbReadService = documentDbReadService;
        }

        // GET api/Flashcard
        public async Task<HttpResponseMessage> Get(long id)
        {
            var userId = User.GetUserId(false);
            var tTransAction = m_QueueProvider.InsertMessageToTranactionAsync(
                      new StatisticsData4(
                        new StatisticsData4.StatisticItemData
                        {
                            Id = id,
                            Action = (int)StatisticsAction.Flashcard
                        }
                    , userId));

            var tUserValues = m_ZboxReadService.GetUserFlashcardAsync(new GetUserFlashcardQuery(
                    userId, id));
            var tValues = m_DocumentDbReadService.FlashcardAsync(id);
            await Task.WhenAll(tTransAction, tValues, tUserValues);
            var values = tValues.Result;
            if (!values.Publish)
            {
                throw new ArgumentException("Flashcard is not published");
            }
            if (values.IsDeleted)
            {
                throw new ArgumentException("Flashcard is deleted");
            }
            return Request.CreateResponse(new
            {
                values.Cards,
                values.Name,
                //values.UserId,
                tUserValues.Result.Pins,
                tUserValues.Result.Like,
                tUserValues.Result.OwnerName
                //tUserValues.Result.UniversityData

            });
            //return "Hello from custom controller!";
        }
    }
}
