using System;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;

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
            if (message == null) throw new ArgumentNullException("message");
            var user = m_UserReposiory.Get(message.UserId);
            if (user == null)
            {
                throw new NullReferenceException("user");
            }

            switch (message.FirstTimeStage)
            {
                case Infrastructure.Enums.FirstTime.Dashboard:
                    user.UpdateDashboardFirstTime();
                    break;
                case Infrastructure.Enums.FirstTime.Library:
                    user.UpdateLibraryFirstTime();
                    break;
                case Infrastructure.Enums.FirstTime.Box:
                    user.UpdateBoxFirstTime();
                    break;
                case Infrastructure.Enums.FirstTime.Item:
                    user.UpdateItemFirstTime();
                    break;
                default:
                    throw new ArgumentException("no such stage exists");
                   
            }
            m_UserReposiory.Save(user);
        }
    }
}
