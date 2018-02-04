using System;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteUserFromBoxCommandHandler : ICommandHandler<DeleteUserFromBoxCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBoxRepository _boxRepository;
        private readonly IUserBoxRelRepository m_UserBoxRelRepository;

        public DeleteUserFromBoxCommandHandler(
            IUserRepository userRepository,
            IBoxRepository boxRepository,
            IUserBoxRelRepository userBoxRelRepository)
        {
            _userRepository = userRepository;
            _boxRepository = boxRepository;
            m_UserBoxRelRepository = userBoxRelRepository;
        }

        public void Handle(DeleteUserFromBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            var box = _boxRepository.Load(command.BoxId);
            if (box.Owner.Id == command.UserToDeleteId)
            {
                throw new InvalidOperationException("Cannot remove owner from his own box");
            }
            if (box.Owner.Id != command.UserId && command.UserId != command.UserToDeleteId)
            {
                throw new InvalidOperationException("Only owner can remove users from his box");
            }
            var userBoxRel = m_UserBoxRelRepository.GetUserBoxRelationship(command.UserToDeleteId, box.Id);
            if (userBoxRel != null)
            {
                userBoxRel.User.UserBoxRel.Remove(userBoxRel);
                _userRepository.Save(userBoxRel.User);
                box.CalculateMembers();
                _boxRepository.Save(box);
            }
        }
    }
}