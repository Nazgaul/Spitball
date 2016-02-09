using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateNodeSettingsCommandHandler : ICommandHandler<UpdateNodeSettingsCommand>
    {
        private readonly IRepository<Library> m_LibraryRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        public UpdateNodeSettingsCommandHandler(IRepository<Library> libraryRepository, IUserRepository userRepository, IGuidIdGenerator guidGenerator)
        {
            m_LibraryRepository = libraryRepository;
            m_UserRepository = userRepository;
            m_GuidGenerator = guidGenerator;
        }

        public void Handle(UpdateNodeSettingsCommand message)
        {
            if (message == null) throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(message.NewName))
            {
                throw new NullReferenceException();
            }

            var node = m_LibraryRepository.Load(message.NodeId);
            var user = m_UserRepository.Load(message.UserId);
            if (node.University.Id != user.University.Id)
            {
                throw new UnauthorizedAccessException();
            }

            node.ChangeName(message.NewName);
            if (message.Settings.HasValue)
            {
                node.UpdateSettings(message.Settings.Value, user, m_GuidGenerator.GetId());
            }
            m_LibraryRepository.Save(node);
        }
    }
}
