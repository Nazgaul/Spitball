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
        private readonly IDepartmentRepository m_DepartmentRepository;
        private readonly IUniversityRepository m_UniversityRepository;

        public CreateAcademicBoxCommandHandler(
            IBoxRepository boxRepository,
            IUserRepository userRepository,
            IRepository<UserBoxRel> userBoxRelRepository,
            IAcademicBoxRepository academicRepository,
            IAcademicBoxThumbnailProvider academicBoxThumbnailProvider,
            IBlobProvider blobProvider, IDepartmentRepository departmentRepository, IUniversityRepository universityRepository)
            : base(boxRepository, userRepository, userBoxRelRepository)
        {
            m_AcademicRepository = academicRepository;
            m_AcademicBoxThumbnailProvider = academicBoxThumbnailProvider;
            m_BlobProvider = blobProvider;
            m_DepartmentRepository = departmentRepository;
            m_UniversityRepository = universityRepository;
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
            var universityUser = user.University2;

            if (department.University != universityUser)
            {
                throw new UnauthorizedAccessException("Department is not part of the university");
            }



            var box = m_AcademicRepository.CheckIfExists(academicCommand.CourseCode, department, academicCommand.Professor
                , academicCommand.BoxName);
            if (box != null)
            {
                throw new BoxNameAlreadyExistsException();
            }

            var picturePath = m_AcademicBoxThumbnailProvider.GetAcademicBoxThumbnail();
            box = new AcademicBox(academicCommand.BoxName, department,
                  academicCommand.CourseCode, academicCommand.Professor,
                  picturePath, user, m_BlobProvider.GetThumbnailUrl(picturePath), universityUser);

            box.UserBoxRelationship.Add(new UserBoxRel(user, box, UserRelationshipType.Owner));
            SaveRepositories(user, box);

            box.CalculateMembers();
            m_AcademicRepository.Save(box, true);
            box.GenerateUrl();
            m_AcademicRepository.Save(box);

            department.UpdateNumberOfBoxes(m_DepartmentRepository.GetBoxesInDepartment(department));
            universityUser.UpdateNumberOfBoxes(m_UniversityRepository.GetNumberOfBoxes(universityUser) + 1);

            m_UniversityRepository.Save(universityUser);
            m_DepartmentRepository.Save(department);

            var result = new CreateBoxCommandResult(box, universityUser.UniversityName);

            return result;
        }



    }
}
