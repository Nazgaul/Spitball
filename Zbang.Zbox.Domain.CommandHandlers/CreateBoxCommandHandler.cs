using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using System;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateBoxCommandHandler : ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>
    {
        protected readonly IUserRepository UserRepository;
        protected readonly IRepository<UserBoxRel> UserBoxRelRepository;
        private readonly IBoxRepository m_BoxRepository;

        public CreateBoxCommandHandler(IBoxRepository boxRepository,
            IUserRepository userRepository, IRepository<UserBoxRel> userBoxRel)
        {
            m_BoxRepository = boxRepository;
            UserRepository = userRepository;
            UserBoxRelRepository = userBoxRel;
        }

        public virtual CreateBoxCommandResult Execute(CreateBoxCommand command)
        {
            ValidateCommand(command);

            User user = UserRepository.Get(command.UserId);
            Box box = m_BoxRepository.GetBoxWithSameName(command.BoxName.Trim().ToLower(), user);
            if (box != null)
            {
                throw new BoxNameAlreadyExistsException();
            }
            box = CreateNewBox(command, user);
            var result = new CreateBoxCommandResult(box, user.Name);
            return result;
        }

        private Box CreateNewBox(CreateBoxCommand command, User user)
        {
            var box = new Box(command.BoxName, user, command.PrivacySettings);

            SaveRepositories(user, box);
            box.GenerateUrl();
            m_BoxRepository.Save(box);
            return box;
        }

        protected void SaveRepositories(User user, Box box)
        {
            UserRepository.Save(user);
            m_BoxRepository.Save(box, true);
        }

        protected void ValidateCommand(CreateBoxCommand command)
        {
            if (command.BoxName.Length > Box.NameLength)
            {
                throw new OverflowException("Box Name exceed" + Box.NameLength);
            }
        }
    }
}
