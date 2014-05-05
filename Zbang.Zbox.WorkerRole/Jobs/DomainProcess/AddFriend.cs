using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    public class AddFriend : IDomainProcess
    {
        readonly private IZboxWriteService m_ZboxWriteService;

        public AddFriend(IZboxWriteService zboxService)
        {
            m_ZboxWriteService = zboxService;

        }
        public bool Excecute(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as AddAFriendData;
            Throw.OnNull(parameters, "parameters");


            var command = new AddFriendCommand(parameters.UserId, parameters.FriendId);
            m_ZboxWriteService.AddAFriend(command);
            return true;
        }
    }

   
}
