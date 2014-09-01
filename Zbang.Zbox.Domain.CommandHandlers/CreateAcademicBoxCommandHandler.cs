using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateAcademicBoxCommandHandler : CreateBoxCommandHandler
    {
        private readonly IAcademicBoxRepository m_AcademicRepository;
        private readonly IAcademicBoxThumbnailProvider m_AcademicBoxThumbnailProvider;
        private readonly IBlobProvider m_BlobProvider;
        private readonly IRepository<Department> m_DepartmentRepository;

        public CreateAcademicBoxCommandHandler(
            IBoxRepository boxRepository,
            IUserRepository userRepository,
            IRepository<UserBoxRel> userBoxRelRepository,
            IAcademicBoxRepository academicRepository,
            IAcademicBoxThumbnailProvider academicBoxThumbnailProvider,
            IBlobProvider blobProvider, IRepository<Department> departmentRepository)
            : base(boxRepository, userRepository, userBoxRelRepository)
        {
            m_AcademicRepository = academicRepository;
            m_AcademicBoxThumbnailProvider = academicBoxThumbnailProvider;
            m_BlobProvider = blobProvider;
            m_DepartmentRepository = departmentRepository;
        }
        public override CreateBoxCommandResult Execute(CreateBoxCommand command)
        {
            var academicCommand = command as CreateAcademicBoxCommand;
            if (academicCommand == null)
            {
                throw new InvalidCastException("can't cast CreateBox to CreateAcademicBox");
            }
            if (command.BoxName.Length > Box.NameLength)
            {
                throw new OverflowException("Box Name exceed" + Box.NameLength);
            }

            User user = UserRepository.Load(command.UserId);
            var department = m_DepartmentRepository.Load(academicCommand.DepartmentId);
            //Library library = m_LibraryRepository.Get(academicCommand.NodeId);

            //if (library == null)
            //{
            //    throw new NullReferenceException("library");
            //}

            //if (library.AmountOfNodes > 0)
            //{
            //    throw new ArgumentException("cannot add box to library with nodes");
            //}
            var universityUser =  user.University2;
            //if (!Equals(universityUser, library.University))
            //{
            //    throw new ArgumentException("library user is not user university");
            //}


            var box = m_AcademicRepository.CheckIfExists(academicCommand.CourseCode, department, academicCommand.Professor
                , academicCommand.BoxName);
            if (box != null)
            {
                throw new BoxNameAlreadyExistsException();
            }
            var picturePath = m_AcademicBoxThumbnailProvider.GetAcademicBoxThumbnail();
            box = new AcademicBox(academicCommand.BoxName, department,
                  academicCommand.CourseCode, academicCommand.Professor,  
                  picturePath, user, m_BlobProvider.GetThumbnailUrl(picturePath));

            //m_LibraryRepository.Save(library);
            box.UserBoxRelationship.Add(new UserBoxRel(user, box, UserRelationshipType.Owner));
            if (universityUser.Id != user.Id)
            {
                box.UserBoxRelationship.Add(new UserBoxRel(user, box, UserRelationshipType.Subscribe));
                SaveRepositories(user, box);

            }
            SaveRepositories(user, box);
            box.CalculateMembers();
            m_AcademicRepository.Save(box,true);
            box.GenerateUrl();
            m_AcademicRepository.Save(box);
            var result = new CreateBoxCommandResult(box, universityUser.UniversityName);

            return result;
        }


       
    }
}
