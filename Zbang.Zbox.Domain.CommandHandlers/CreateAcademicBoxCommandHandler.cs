using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateAcademicBoxCommandHandler : CreateBoxCommandHandler
    {
        private readonly IAcademicBoxRepository m_AcademicRepository;
        private readonly ILibraryRepository m_DepartmentRepository;
        private readonly IUniversityRepository m_UniversityRepository;


        public CreateAcademicBoxCommandHandler(
            IBoxRepository boxRepository,
            IUserRepository userRepository,
            IRepository<UserBoxRel> userBoxRelRepository,
            IAcademicBoxRepository academicRepository,
            ILibraryRepository departmentRepository,
            IUniversityRepository universityRepository, IGuidIdGenerator guidGenerator,
            IQueueProvider queueProvider)
            : base(boxRepository, userRepository, userBoxRelRepository, guidGenerator, queueProvider)
        {
            m_AcademicRepository = academicRepository;
            m_DepartmentRepository = departmentRepository;
            m_UniversityRepository = universityRepository;

        }
        public override async Task<CreateBoxCommandResult> ExecuteAsync(CreateBoxCommand command)
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
            var department = m_DepartmentRepository.Load(academicCommand.DepartmentId);
            var box = m_AcademicRepository.CheckIfExists(academicCommand.CourseCode, department, academicCommand.Professor
                , academicCommand.BoxName);
            if (box != null)
            {
                throw new BoxNameAlreadyExistsException();
            }
            var user = UserRepository.Load(command.UserId);
            if (department.University != user.University && department.University != user.University.UniversityData)
            {
                throw new UnauthorizedAccessException("Department is not part of the university");
            }
            if (department.Settings == LibraryNodeSetting.Closed)
            {
                var topDepartmentId = m_DepartmentRepository.GetTopTreeNode(academicCommand.DepartmentId);
                var topDepartment = m_DepartmentRepository.Load(topDepartmentId);
                box = new AcademicBoxClosed(academicCommand.BoxName, department,
                    academicCommand.CourseCode, academicCommand.Professor,
                    user, GuidGenerator.GetId(), topDepartment);
            }
            else
            {
                box = new AcademicBox(academicCommand.BoxName, department,
                    academicCommand.CourseCode, academicCommand.Professor,
                    user, GuidGenerator.GetId());
            }
            box.UserBoxRelationship.Add(new UserBoxRel(user, box, UserRelationshipType.Owner));
            SaveRepositories(user, box);

            box.CalculateMembers();
            m_AcademicRepository.Save(box, true);
            box.GenerateUrl();
            m_AcademicRepository.Save(box);

            var countOfBoxes = m_UniversityRepository.GetNumberOfBoxes(user.University.UniversityData) + 1;
            m_DepartmentRepository.Save(department.UpdateNumberOfBoxes());
            // department.UpdateNumberOfBoxes(m_DepartmentRepository.GetBoxesInDepartment(department));
            user.University.UpdateNumberOfBoxes(countOfBoxes);
            user.University.UniversityData.UpdateNumberOfBoxes(countOfBoxes);

            m_UniversityRepository.Save(user.University);
            m_UniversityRepository.Save(user.University.UniversityData);
            await QueueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id));

            var result = new CreateBoxCommandResult(box.Id, box.Url);

            return result;
        }



    }
}
