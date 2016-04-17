using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class NoFollowClassMailProcess : IMailProcess
    {
        public Task<bool> ExcecuteAsync(int index, Action<int> progress, CancellationToken token)
        {
            return Task.FromResult(true);
            //throw new NotImplementedException();
        }
    }
}
