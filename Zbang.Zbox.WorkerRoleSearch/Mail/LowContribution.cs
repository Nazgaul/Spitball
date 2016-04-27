﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class LowContribution : BaseMarketingMailProcess
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        public LowContribution(IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService) : base(mailComponent)
        {
            m_ZboxReadService = zboxReadService;
        }

        protected override Task<IEnumerable<MarketingDto>> GetDataAsync(MarketingQuery query, CancellationToken token)
        {
            return m_ZboxReadService.GetLowContributersUsersAsync(query, token);
        }

        protected override MarketingMailParams BuildMarkertingMail(string name, CultureInfo info)
        {
            return new LowContributionMailParams(name, info);
        }

        protected override string ServiceName =>nameof(LowContribution );
    }
}
