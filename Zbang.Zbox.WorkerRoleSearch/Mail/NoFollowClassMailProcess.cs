using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class NoFollowClassMailProcess : IMailProcess
    {
        private readonly IMailComponent m_MailComponent;

        public NoFollowClassMailProcess(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public Task<bool> ExcecuteAsync(int index, Action<int> progress, CancellationToken token)
        {
            
            return Task.FromResult(true);
            //throw new NotImplementedException();
        }
    }
}
