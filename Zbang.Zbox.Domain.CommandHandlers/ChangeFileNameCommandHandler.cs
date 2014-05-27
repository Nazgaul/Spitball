using System;
using System.IO;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
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
            var user = m_UserRepository.Load(command.UserId);
            var item = m_ItemRepository.Get(command.FileId);

            var newName = item.ChangeName(command.NewFileName);

            //var fileNameExtension = Path.GetExtension(item.Name);
            //var fileNameWithoutExtension = command.NewFileName.RemoveEndOfString(Item.NameLength);
            //if (fileNameWithoutExtension == Path.GetFileNameWithoutExtension(item.Name))
            //{
            //    return new ChangeFileNameCommandResult(item);
            //}

            //if (!Validation.IsValidFileName(command.NewFileName))
            //{
            //    throw new ArgumentException("file name is not a valid file name", "NewFileName");
            //}

            //var newUniquefileName = item.Box.GetUniqueFileName(fileNameWithoutExtension + fileNameExtension);// command.NewFileName);

            //item.Name = newUniquefileName;
            item.DateTimeUser.UpdateUserTime(user.Email);
            m_ItemRepository.Save(item);

            return new ChangeFileNameCommandResult(newName);
        }
    }
}
