﻿using System;
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
        private readonly IRepository<User> m_UserRepository;
        private readonly IRepository<Department> m_DepartmentRepository;

        public CreateDepartmentCommandHandler(IIdGenerator idGenerator, IRepository<University> universityRepository, IRepository<Department> departmentRepository, IRepository<User> userRepository)
        {
            m_IdGenerator = idGenerator;
            m_UniversityRepository = universityRepository;
            m_DepartmentRepository = departmentRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(CreateDepartmentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var university = m_UniversityRepository.Get(message.UniversityId);//we need get in order to get uni name in ctor
            var user = m_UserRepository.Load(message.UserId); 


            var newId = m_IdGenerator.GetId(IdGenerator.DepartmentScope);
            var department = new Department(newId, message.Name, university);

            user.Department = department;
            message.Id = newId;
            m_DepartmentRepository.Save(department);
            m_UserRepository.Save(user);
        }
    }


}
