using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class NoUniversityMailProcess : BaseMarketingMailProcess
    {
        private readonly IZboxReadServiceWorkerRole _zboxReadService;

        public NoUniversityMailProcess(ILogger logger, IZboxReadServiceWorkerRole zboxReadService, IMailComponent mailComponent)
            : base(mailComponent, logger)
        {
            _zboxReadService = zboxReadService;
        }

        protected override Task<IEnumerable<MarketingDto>> GetDataAsync(MarketingQuery query, CancellationToken token)
        {
            return _zboxReadService.GetUsersWithoutUniversityAsync(query, token);
        }

        protected override MarketingMailParams BuildMarketingMail(string name, CultureInfo info)
        {
            return new NoUniversityMailParams(name, info);
        }

        protected override string ServiceName => "no university";
    }
}
