using Cloudents.Core.Message.Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;

namespace Cloudents.Core.Interfaces
{
    public interface IMondayProvider
    {
        Task CreateRecordAsync(MondayMessage email, CancellationToken token);
    }
}
