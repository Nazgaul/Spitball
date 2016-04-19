using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Emails;
using Zbang.Zbox.ViewModel.Queries.Emails;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class NoUniversityMailProcess : BaseMarketingMailProcess
    {
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;

        public NoUniversityMailProcess(IZboxReadServiceWorkerRole zboxReadService, IMailComponent mailComponent)
            :base (mailComponent)
        {
            m_ZboxReadService = zboxReadService;
        }


//        public async Task<bool> ExcecuteAsync(int index, Action<int> progress, CancellationToken token)
//        {
//            var page = index;
//            var needToContinueRun = true;
//            var list = new List<Task>();
//            while (needToContinueRun)
//            {
//                TraceLog.WriteInfo($"running no university mail page {page}");
//                needToContinueRun = false;
//                var result = await m_ZboxReadService.GetUsersWithoutUniversityAsync(
//                        new ViewModel.Queries.Emails.MarketingQuery(page, 100), token);
//                foreach (var user in result)
//                {
//                    needToContinueRun = true;
//                    var email = user.Email;
//#if DEBUG
//                    //email = "ram@cloudents.com";
//#endif
//                    var culture = string.IsNullOrEmpty(user.Culture)
//                        ? Thread.CurrentThread.CurrentCulture
//                        : new System.Globalization.CultureInfo(user.Culture);

//                    list.Add( m_MailComponent.GenerateAndSendEmailAsync(email,
//                        new NoUniversityMailParams(user.Name, culture), token));
//                }
//                await Task.WhenAll(list);
//                list.Clear();
//                page++;
//                progress(page);
                
//            }
//            TraceLog.WriteInfo($"finish running no university mail page {index}");
//            return true;

//        }
        protected override async Task<IEnumerable<MarketingDto>> GetDataAsync(MarketingQuery query, CancellationToken token)
        {
            return await m_ZboxReadService.GetUsersWithoutUniversityAsync(query, token);
        }

        protected override MarketingMailParams BuildMarkertingMail(string name, CultureInfo info)
        {
            return new NoUniversityMailParams(name, info);
        }

        protected override string ServiceName => "no university";
    }
}
