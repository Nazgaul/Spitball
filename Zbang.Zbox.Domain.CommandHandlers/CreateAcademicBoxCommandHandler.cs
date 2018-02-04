using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateAcademicBoxCommandHandler : CreateBoxCommandHandler
    {
        private readonly IAcademicBoxRepository _academicRepository;
        private readonly ILibraryRepository _departmentRepository;
        private readonly IUniversityRepository _universityRepository;

        public CreateAcademicBoxCommandHandler(
            IBoxRepository boxRepository,
            IUserRepository userRepository,
            IAcademicBoxRepository academicRepository,
            ILibraryRepository departmentRepository,
            IUniversityRepository universityRepository, IGuidIdGenerator guidGenerator,
            IQueueProvider queueProvider)
            : base(boxRepository, userRepository,  guidGenerator, queueProvider)
        {
            _academicRepository = academicRepository;
            _departmentRepository = departmentRepository;
            _universityRepository = universityRepository;
        }

        public override async Task<CreateBoxCommandResult> ExecuteAsync(CreateBoxCommand command)
        {
            if (!(command is CreateAcademicBoxCommand academicCommand))
            {
                throw new InvalidCastException("can't cast CreateBox to CreateAcademicBox");
            }
            if (command.BoxName.Length > Box.NameLength)
            {
                throw new OverflowException("Box Name exceed" + Box.NameLength);
            }
            var department = _departmentRepository.Load(academicCommand.DepartmentId);
            var box = _academicRepository.CheckIfExists(academicCommand.CourseCode, department, academicCommand.Professor
                , academicCommand.BoxName);
            if (box != null)
            {
                throw new BoxNameAlreadyExistsException(box.Id);
            }
            var user = UserRepository.Load(command.UserId);
            if (department.University != user.University && department.University != user.University.UniversityData)
            {
                throw new UnauthorizedAccessException("Department is not part of the university");
            }
            if (department.Settings == LibraryNodeSetting.Closed)
            {
                var topDepartmentId = _departmentRepository.GetTopTreeNode(academicCommand.DepartmentId);
                var topDepartment = _departmentRepository.Load(topDepartmentId);
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
            _academicRepository.Save(box, true);
            box.GenerateUrl();
            _academicRepository.Save(box);

            var countOfBoxes = _universityRepository.GetNumberOfBoxes(user.University.UniversityData) + 1;
            _departmentRepository.Save(department.UpdateNumberOfBoxes());
            user.University.UpdateNumberOfBoxes(countOfBoxes);
            user.University.UniversityData.UpdateNumberOfBoxes(countOfBoxes);

            _universityRepository.Save(user.University);
            _universityRepository.Save(user.University.UniversityData);
            await QueueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id)).ConfigureAwait(true);

            return new CreateBoxCommandResult(box.Id, box.Url);
        }
    }
}
