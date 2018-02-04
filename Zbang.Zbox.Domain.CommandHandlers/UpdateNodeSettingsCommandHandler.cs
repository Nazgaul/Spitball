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
        private readonly IRepository<Library> _libraryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGuidIdGenerator _guidGenerator;
        public UpdateNodeSettingsCommandHandler(IRepository<Library> libraryRepository, IUserRepository userRepository, IGuidIdGenerator guidGenerator)
        {
            _libraryRepository = libraryRepository;
            _userRepository = userRepository;
            _guidGenerator = guidGenerator;
        }

        public void Handle(UpdateNodeSettingsCommand message)
        {
            if (message == null) throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(message.NewName))
            {
                throw new NullReferenceException();
            }

            var node = _libraryRepository.Load(message.NodeId);
            var user = _userRepository.Load(message.UserId);
            if (node.University.Id != user.University.Id)
            {
                throw new UnauthorizedAccessException();
            }

            node.ChangeName(message.NewName);
            if (message.Settings.HasValue)
            {
                //if (message.UserId != node.CreatedUser.Id)
                //{
                //    throw new UnauthorizedAccessException();
                //}
                node.UpdateSettings(message.Settings.Value, user, _guidGenerator.GetId());
            }
            _libraryRepository.Save(node);
        }
    }
}
