using System;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Storage.Entities;
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
        public bool Excecute(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as BadItemData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            m_TableProvider.InsertUserRequestAsync(
                new FlagItem(parameters.ItemId, parameters.UserId, parameters.Other, parameters.Reason));

            var flagItemDetail = m_ZboxReadService.GetFlagItemUserDetail(new GetBadItemFlagQuery(parameters.UserId, parameters.ItemId));

            m_MailComponent.GenerateAndSendEmail("eidan@cloudents.com",
                new FlagItemMailParams(flagItemDetail.ItemName,
                    string.Format("{0} {1}", parameters.Reason, parameters.Other),
                    flagItemDetail.Name,
                    flagItemDetail.Email,
                    string.Empty
                    //string.Format(UrlConsts.ItemUrl, flagItemDetail.BoxId, flagItemDetail.Uid )
                    ));
            return true;
        }
    }
}
