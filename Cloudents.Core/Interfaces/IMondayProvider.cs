using Cloudents.Core.Message.Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IMondayProvider
    {
        Task CreateRecordAsync(RequestTutorEmail email, CancellationToken token);
    }
}
