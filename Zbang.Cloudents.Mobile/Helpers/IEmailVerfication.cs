using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public interface IEmailVerfication
    {
        Task<bool> VerifyEmailAsync(string email);
    }
}
