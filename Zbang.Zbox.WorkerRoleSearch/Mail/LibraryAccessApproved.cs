using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class LibraryAccessApproved : IMail2
    {
        private readonly IMailComponent _mailComponent;

        public LibraryAccessApproved(IMailComponent mailComponent)
        {
            _mailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is AccessApprovedData parameters))
            {
                throw new NullReferenceException("parameters");
            }
            //string depName = parameters.DepName;

            await _mailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                new DepartmentApprovedMailParams(new CultureInfo(parameters.Culture), parameters.DepName), token).ConfigureAwait(false);

            return true;
        }
    }
}
