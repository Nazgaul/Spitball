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
   public  class AddReputationCommandHandler : ICommandHandler<AddReputationCommand>
    {
       private readonly IUserRepository m_UserRepository;
       public AddReputationCommandHandler(IUserRepository userRepository)
       {
           m_UserRepository = userRepository;
       }
       public void Handle(AddReputationCommand message)
        {
            var user = m_UserRepository.Load(message.UserId);
            user.AddReputation(5);
            m_UserRepository.Save(user);
        }
    }
}
