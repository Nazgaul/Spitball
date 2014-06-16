using System;
using System.Linq;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UnfollowBoxCommandHandler : ICommandHandler<UnfollowBoxCommand>
    {
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;

        public UnfollowBoxCommandHandler(IRepository<Box> boxRepository,
            IUserRepository userRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(UnfollowBoxCommand message)
        {
            var box = m_BoxRepository.Get(message.BoxId);
            var user = m_UserRepository.Load(message.UserId);
            if (box == null || box.IsDeleted)
            {
                throw new BoxDoesntExistException();
            }
            if (box.CommentCount <= 1 && box.MembersCount <= 2 && box.ItemCount == 0 && box.UserTime.CreatedUser == user.Email)
            {
                DeleteBox(box);
                return;
            }
            UserRelationshipType userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, box.Id);
            if (userType == UserRelationshipType.Owner)
            {
                DeleteBox(box);
                return;
            }
            if (userType == UserRelationshipType.Subscribe)
            {
                UnfollowBox(box, message.UserId);
            }


        }

        private void DeleteBox(Box box)
        {
            var acadmicBox = box as AcademicBox;
            if (acadmicBox != null)
            {
                foreach (var library in acadmicBox.Library)
                {
                    library.Boxes.Remove(box);

                }
            }
            var users = box.UserBoxRel.Select(s => s.User);
            foreach (var userInBox in users)
            {
                //userInBox.ReCalculateSpace();
                userInBox.Quota.UsedSpace = m_UserRepository.GetItemsByUser(userInBox.Id).Sum(s => s.Size);
                m_UserRepository.Save(userInBox);
            }
            box.UserBoxRel.Clear();
            box.IsDeleted = true;
            box.UserTime.UpdateUserTime(box.Owner.Email);
            m_BoxRepository.Save(box);
        }
        private void UnfollowBox(Box box, long userId)
        {
            var userBoxRel = box.UserBoxRel.FirstOrDefault(w => w.User.Id == userId);
            if (userBoxRel == null) //TODO: this happen when user decline invite of a box that is public
            {
                throw new InvalidOperationException("User does not have an active invite");
            }

            box.UserBoxRel.Remove(userBoxRel);

            // var box = m_BoxRepository.Get(command.BoxId);
            box.CalculateMembers();
            m_BoxRepository.Save(box);
        }
    }
}
