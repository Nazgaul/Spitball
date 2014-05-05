using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;

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
            Throw.OnNull(parameters, "parameters");
            m_TableProvider.InsertUserRequestAsync(
                new Zbox.Infrastructure.Storage.Entities.FlagItem(parameters.ItemId, parameters.UserId, parameters.Other, parameters.Reason));

            var flagItemDetail = m_ZboxReadService.GetFlagItemUserDetail(new ViewModel.Queries.Emails.GetBadItemFlagQuery(parameters.UserId, parameters.ItemId));

            m_MailComponent.GenerateAndSendEmail("eidan@cloudents.com",
                new FlagItemMailParams(flagItemDetail.ItemName,
                    string.Format("{0} {1}", parameters.Reason, parameters.Other),
                    flagItemDetail.Name,
                    flagItemDetail.Email,
                    string.Format(UrlConsts.ItemUrl, flagItemDetail.BoxUid, flagItemDetail.Uid)));
            return true;
        }
    }
}
