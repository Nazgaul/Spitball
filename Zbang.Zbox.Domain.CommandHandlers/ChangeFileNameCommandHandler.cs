using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    class ChangeFileNameCommandHandler : ICommandHandler<ChangeFileNameCommand, ChangeFileNameCommandResult>
    {
        readonly IUserRepository m_UserRepository;
        readonly IRepository<Item> m_ItemRepository;
        public ChangeFileNameCommandHandler(IUserRepository userRepository, IRepository<Item> itemRepository)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
        }
        public ChangeFileNameCommandResult Execute(ChangeFileNameCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var user = m_UserRepository.Load(command.UserId);
            var item = m_ItemRepository.Get(command.FileId);

            var newName = item.ChangeName(command.NewFileName);

           item.DateTimeUser.UpdateUserTime(user.Email);
            m_ItemRepository.Save(item);

            return new ChangeFileNameCommandResult(newName);
        }
    }
}
