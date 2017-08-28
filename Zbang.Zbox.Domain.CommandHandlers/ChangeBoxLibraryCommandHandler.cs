using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class ChangeBoxLibraryCommandHandler : ICommandHandler<ChangeBoxLibraryCommand>
    {
        private readonly IRepository<Library> m_DepartmentRepository;
        private readonly IRepository<AcademicBox> m_BoxRepository;

        public ChangeBoxLibraryCommandHandler(IRepository<Library> departmentRepository,
            IRepository<AcademicBox> boxRepository)
        {
            m_DepartmentRepository = departmentRepository;
            m_BoxRepository = boxRepository;
        }

        public void Handle(ChangeBoxLibraryCommand message)
        {
            var department = m_DepartmentRepository.Load(message.LibraryId);
            foreach (var boxId in message.BoxIds)
            {
                var box = m_BoxRepository.Load(boxId);
                var oldDepartment = box.Department;
                box.Department = department;
                m_BoxRepository.Save(box, true);
                m_DepartmentRepository.Save(oldDepartment.UpdateNumberOfBoxes());
            }
            m_DepartmentRepository.Save(department.UpdateNumberOfBoxes());
        }
    }
}
