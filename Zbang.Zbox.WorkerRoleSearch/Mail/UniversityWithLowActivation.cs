using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class UniversityWithLowActivation : BaseMarketingMailProcess
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        public UniversityWithLowActivation(ILogger logger, IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService) : base(mailComponent, logger)
        {
            m_ZboxReadService = zboxReadService;
        }

        protected override Task<IEnumerable<MarketingDto>> GetDataAsync(MarketingQuery query, CancellationToken token)
        {
            return m_ZboxReadService.GetUsersWithLowActivityUniversitiesAsync(query, token);
        }

        protected override MarketingMailParams BuildMarketingMail(string name, CultureInfo info)
        {
            return new UniversityLowActivityMailParams(name, info);
        }

        protected override string ServiceName => "University with low activity";
    }
}
