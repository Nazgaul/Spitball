using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserFirstTimeStatusCommandHandler:ICommandHandler<UpdateUserFirstTimeStatusCommand>
    {
        private readonly IUserRepository m_UserReposiory;
        public UpdateUserFirstTimeStatusCommandHandler(IUserRepository userRepository)
        {
            m_UserReposiory = userRepository;
        }
        public void Handle(UpdateUserFirstTimeStatusCommand message)
        {
            var user = m_UserReposiory.Get(message.UserId);
            Throw.OnNull(user, "user");

            switch (message.FirstTimeStage)
            {
                case Zbang.Zbox.Infrastructure.Enums.FirstTime.Dashboard:
                    user.UpdateDashboardFirstTime();
                    break;
                case Zbang.Zbox.Infrastructure.Enums.FirstTime.Library:
                    user.UpdateLibraryFirstTime();
                    break;
                case Zbang.Zbox.Infrastructure.Enums.FirstTime.Box:
                    user.UpdateBoxFirstTime();
                    break;
                case Zbang.Zbox.Infrastructure.Enums.FirstTime.Item:
                    user.UpdateItemFirstTime();
                    break;
                default:
                    throw new ArgumentException("no such stage exists");
                   
            }
            m_UserReposiory.Save(user);
        }
    }
}
