using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Entities;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    public class FlagBadItem : IDomainProcess
    {
        private readonly IMailComponent m_MailComponent;
        private readonly ITableProvider m_TableProvider;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;


        public FlagBadItem(IMailComponent mailComponent, ITableProvider tableProvider, IZboxReadServiceWorkerRole zboxReadService)
        {
            m_MailComponent = mailComponent;
            m_TableProvider = tableProvider;
            m_ZboxReadService = zboxReadService;
        }
        public Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as BadItemData;
            if (parameters == null)
            {
                var parameters2 = data as BadPostData;
                if (parameters2 == null)
                {
                    throw new ArgumentNullException("data");

                }
                return FlagPost(parameters2);
            }
            return FlagItem(parameters);

        }

        private async Task<bool> FlagItem(BadItemData parameters)
        {
            await m_TableProvider.InsertUserRequestAsync(
                new FlagItem(parameters.ItemId, parameters.UserId, parameters.Other, parameters.Reason));

            var flagItemDetail = m_ZboxReadService.GetFlagItemUserDetail(new GetBadItemFlagQuery(parameters.UserId, parameters.ItemId));

            m_MailComponent.GenerateAndSendEmail("eidan@cloudents.com",
                new FlagItemMailParams(flagItemDetail.ItemName,
                    string.Format("{0} {1}", parameters.Reason, parameters.Other),
                    flagItemDetail.Name,
                    flagItemDetail.Email,
                    string.Empty
                    ));
            return true;
        }

        private async Task<bool> FlagPost(BadPostData parameters)
        {
            await m_TableProvider.InsertUserRequestAsync(
                 new FlagCommentOrReply(parameters.PostId, parameters.UserId));


            m_MailComponent.GenerateAndSendEmail("eidan@cloudents.com",
                new FlagItemMailParams("post or reply", " from ios app", "dont know", "dont know", string.Empty));
            return true;
        }
    }
}
