using System;
using System.Linq;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UnFollowBoxCommandHandler : ICommandHandler<UnFollowBoxCommand>
    {
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Library> m_DepartmentRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IUniversityRepository m_UniversityRepository;
        private readonly IRepository<UserBoxRel> m_UserBoxRelRepository;

        public UnFollowBoxCommandHandler(IRepository<Box> boxRepository,
            IUserRepository userRepository,
            IRepository<Library> departmentRepository,
            IUniversityRepository universityRepository, IQueueProvider queueProvider, IRepository<UserBoxRel> userBoxRelRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_DepartmentRepository = departmentRepository;
            m_UniversityRepository = universityRepository;
            m_QueueProvider = queueProvider;
            m_UserBoxRelRepository = userBoxRelRepository;
        }

        public void Handle(UnFollowBoxCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var box = m_BoxRepository.Get(message.BoxId);
            var user = m_UserRepository.Load(message.UserId);

            if (user.Reputation > user.University.AdminScore && message.ShouldDelete)
            {
                DeleteBox(box);
                return;
            }

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, message.BoxId);
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
            //box.IsDeleted = true;
            box.UserTime.UpdateUserTime(box.Owner.Email);
            var academicBox = box as AcademicBox;
            var users = box.UserBoxRelationship.Select(s => s.User.Id).ToList();
           

            if (academicBox != null)
            {
                var university = academicBox.University;
                var department = academicBox.Department;
                var noOfBoxes = m_UniversityRepository.GetNumberOfBoxes(university);
                m_BoxRepository.Delete(box);
                m_DepartmentRepository.Save(department.UpdateNumberOfBoxes());
                university.UpdateNumberOfBoxes(--noOfBoxes);
                m_UniversityRepository.Save(university);
            }
            
            
            m_BoxRepository.Delete(box);
            m_QueueProvider.InsertMessageToTranaction(new QuotaData(users));
            m_QueueProvider.InsertMessageToTranaction(new ReputationData(users));

            //m_BoxRepository.Save(box);
        }
        private void UnFollowBox(Box box, long userId)
        {
            box.UnFollowBox(userId);
            m_BoxRepository.Save(box);
        }
    }
}
