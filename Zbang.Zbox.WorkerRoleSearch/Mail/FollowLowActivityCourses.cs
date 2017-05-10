using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class FollowLowActivityCourses : BaseMarketingMailProcess
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        public FollowLowActivityCourses(IMailComponent mailComponent, IZboxReadServiceWorkerRole zboxReadService) : base(mailComponent)
        {
            m_ZboxReadService = zboxReadService;
        }

        protected override Task<IEnumerable<MarketingDto>> GetDataAsync(MarketingQuery query, CancellationToken token)
        {
            return m_ZboxReadService.GetUsersFollowingCoursesNoActivityAsync(query, token);
        }

        protected override MarketingMailParams BuildMarketingMail(string name, CultureInfo info)
        {
            return new LowCoursesActivityMailParams(name, info);
        }

        protected override string ServiceName => "Follow low activity courses";
    }
}
