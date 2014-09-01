using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateDepartmentCommandHandler : ICommandHandler<CreateDepartmentCommand>
    {
        private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<Department> m_DepartmentRepository;

        public CreateDepartmentCommandHandler(IIdGenerator idGenerator, IRepository<University> universityRepository, IRepository<Department> departmentRepository)
        {
            m_IdGenerator = idGenerator;
            m_UniversityRepository = universityRepository;
            m_DepartmentRepository = departmentRepository;
        }

        public void Handle(CreateDepartmentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var university = m_UniversityRepository.Load(message.UniversityId);

            var newId = m_IdGenerator.GetId(IdGenerator.DepartmentScope);
            var department = new Department(newId, message.Name, university);

            message.Id = newId;
            m_DepartmentRepository.Save(department);
        }
    }


}
