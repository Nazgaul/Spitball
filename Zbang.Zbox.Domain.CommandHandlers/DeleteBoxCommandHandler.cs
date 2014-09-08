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
    public class DeleteBoxCommandHandler : ICommandHandler<DeleteBoxCommand>
    {

        private readonly IRepository<Box> m_BoxRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IDepartmentRepository m_DepartmentRepository;
        private readonly IUniversityRepository m_UniversityRepository;

        public DeleteBoxCommandHandler(IRepository<Box> boxRepository,
            IUserRepository userRepository, IDepartmentRepository departmentRepository, IUniversityRepository universityRepository)
        {
            m_BoxRepository = boxRepository;
            m_UserRepository = userRepository;
            m_DepartmentRepository = departmentRepository;
            m_UniversityRepository = universityRepository;
        }

        public void Handle(DeleteBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            Box box = m_BoxRepository.Get(command.BoxId);
            if (box == null || box.IsDeleted)
            {
                throw new BoxDoesntExistException();
            }


            UserRelationshipType userType = m_UserRepository.GetUserToBoxRelationShipType(command.UserId, box.Id); //user.GetUserType(box.Id);


            //if box is empty everyone can remove it only the owner and another user
            if (box.CommentCount == 1 && box.MembersCount <= 2 && box.ItemCount == 0)
            {
                userType = UserRelationshipType.Owner;
            }


            //Authorize
            if (userType != UserRelationshipType.Owner)
            {
                throw new UnauthorizedAccessException("User cannot delete this box");
            }


            var academicBox = box as AcademicBox;
            if (academicBox != null)
            {
                var department = academicBox.Department;
                var noOfBoxes = m_DepartmentRepository.GetBoxesInDepartment(department);
                department.UpdateNumberOfBoxes(--noOfBoxes);
                m_DepartmentRepository.Save(department);

                var university = academicBox.University;
                noOfBoxes = m_UniversityRepository.GetNumberOfBoxes(university);
                university.UpdateNumberOfBoxes(--noOfBoxes);
                m_UniversityRepository.Save(university);
                //    foreach (var library in acadmicBox.Library)
                //    {
                //        library.Boxes.Remove(box);

                //    }
            }
            //Delete blobs

            //foreach (var item in box.Items)
            //{
            //    item.IsDeleted = true;
            //    var file = item as File;
            //    if (file == null) continue;

            //    //m_BlobProvider.DeleteFile(file.BlobName);               
            //    //TODO: We need to delete blob thumbnail as well
            //}

            var users = box.UserBoxRelationship.Select(s => s.User);
            foreach (var userInBox in users)
            {
                //userInBox.ReCalculateSpace();
                userInBox.Quota.UsedSpace = m_UserRepository.GetItemsByUser(userInBox.Id).Sum(s => s.Size);
                m_UserRepository.Save(userInBox);
            }

            box.IsDeleted = true;
            User user = m_UserRepository.Load(command.UserId);
            box.UserTime.UpdateUserTime(user.Email);
            m_BoxRepository.Save(box);
        }
    }
}
