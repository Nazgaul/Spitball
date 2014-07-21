using System;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteUserFromBoxCommandHandler : ICommandHandler<DeleteUserFromBoxCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IUserBoxRelRepository m_UserBoxRelRepository;

        public DeleteUserFromBoxCommandHandler(
            IUserRepository userRepository,
            IBoxRepository boxRepository,
            IUserBoxRelRepository userBoxRelRepository)
        {
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_UserBoxRelRepository = userBoxRelRepository;

        }
        public void Handle(DeleteUserFromBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var box = m_BoxRepository.Load(command.BoxId);
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
                m_UserRepository.Save(userBoxRel.User);
                box.CalculateMembers();
                m_BoxRepository.Save(box);
            }
            var user = m_UserRepository.Get(command.UserToDeleteId); // we using get because load raise exception on save
            user.RemoveInviteState(box);
            m_UserRepository.Save(user);
        }
    }
}