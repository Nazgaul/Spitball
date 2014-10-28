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
        private readonly IRepository<Library> m_DepartmentRepository;
        private readonly IUniversityRepository m_UniversityRepository;

        public UnfollowBoxCommandHandler(IRepository<Box> boxRepository,
            IUserRepository userRepository,
            IRepository<Library> departmentRepository, 
            IUniversityRepository universityRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_DepartmentRepository = departmentRepository;
            m_UniversityRepository = universityRepository;
        }

        public void Handle(UnfollowBoxCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var box = m_BoxRepository.Get(message.BoxId);
            if (box == null || box.IsDeleted)
            {
                throw new BoxDoesntExistException();
            }
            if (box.CommentCount <= 1 && box.MembersCount <= 2 && box.ItemCount == 0)
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
                UnFollowBox(box, message.UserId);
            }


        }

        private void DeleteBox(Box box)
        {
            box.UserBoxRelationship.Clear();
            box.IsDeleted = true;
            box.UserTime.UpdateUserTime(box.Owner.Email);
            var academicBox = box as AcademicBox;
            if (academicBox != null)
            {
                var university = academicBox.University;
                var department = academicBox.Department;
                var noOfBoxes = m_UniversityRepository.GetNumberOfBoxes(university);
                department.UpdateNumberOfBoxes();
                university.UpdateNumberOfBoxes(--noOfBoxes);
                while (department != null)
                {
                    m_DepartmentRepository.Save(department);
                    department = department.Parent;
                }
                m_UniversityRepository.Save(university);
            }
            var users = box.UserBoxRelationship.Select(s => s.User);
            foreach (var userInBox in users)
            {
                userInBox.Quota.UsedSpace = m_UserRepository.GetItemsByUser(userInBox.Id).Sum(s => s.Size);
                m_UserRepository.Save(userInBox);
            }
           
            m_BoxRepository.Save(box);
        }
        private void UnFollowBox(Box box, long userId)
        {
            var userBoxRel = box.UserBoxRelationship.FirstOrDefault(w => w.User.Id == userId);
            if (userBoxRel == null) //TODO: this happen when user decline invite of a box that is public
            {
                throw new InvalidOperationException("User does not have an active invite");
            }

            box.UserBoxRelationship.Remove(userBoxRel);

            // var box = m_BoxRepository.Get(command.BoxId);
            box.CalculateMembers();
            m_BoxRepository.Save(box);
        }
    }
}
