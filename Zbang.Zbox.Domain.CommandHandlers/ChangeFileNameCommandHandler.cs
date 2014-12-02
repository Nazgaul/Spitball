using System;
using System.IO;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeFileNameCommandHandler : ICommandHandler<ChangeFileNameCommand, ChangeFileNameCommandResult>
    {
        readonly IUserRepository m_UserRepository;
        readonly IItemRepository m_ItemRepository;
        public ChangeFileNameCommandHandler(IUserRepository userRepository, IItemRepository itemRepository)
        {
            m_UserRepository = userRepository;
            m_ItemRepository = itemRepository;
        }
        public ChangeFileNameCommandResult Execute(ChangeFileNameCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var user = m_UserRepository.Load(command.UserId);
            var item = m_ItemRepository.Get(command.FileId);
            var fileName = GetUniqueFileNameToBox(command.NewFileName, item.Box);
            var newName = item.ChangeName(fileName);

            item.DateTimeUser.UpdateUserTime(user.Email);
            m_ItemRepository.Save(item);

            return new ChangeFileNameCommandResult(newName, item.Url);
        }


        private string GetUniqueFileNameToBox(string fileName, Box box)
        {
            var origFileName = fileName.RemoveEndOfString(Item.NameLength);
            var fileExists = m_ItemRepository.CheckFileNameExists(origFileName, box);

            if (fileExists)
            {
                var index = 0;
                //Find next available index
                do
                {
                    index++;
                    fileName = string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(origFileName), index,
                        Path.GetExtension(fileName));
                    fileExists = m_ItemRepository.CheckFileNameExists(fileName, box);
                } while (fileExists);
            }
            return fileName;
        }
    }
}
