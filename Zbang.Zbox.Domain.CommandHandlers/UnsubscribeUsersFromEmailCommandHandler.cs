using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UnsubscribeUsersFromEmailCommandHandler : ICommandHandler<UnsubscribeUsersFromEmailCommand>
    {
        private readonly IUserRepository m_UserRepository;

        public UnsubscribeUsersFromEmailCommandHandler(IUserRepository userRepository)
        {
            m_UserRepository = userRepository;
        }

        public void Handle(UnsubscribeUsersFromEmailCommand message)
        {
            m_UserRepository.UnsubscibeUserFromMail(message.Emails,message.Type);
        }
    }
}
