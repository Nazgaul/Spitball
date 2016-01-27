﻿using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddStudentCommandHandler : ICommandHandler<AddStudentCommand>
    {
        private readonly IRepository<Student> m_StudentRepository;
        private readonly IRepository<University> m_UniversityRepository;
        public AddStudentCommandHandler(IRepository<Student> studentRepository, IRepository<University> universityRepository)
        {
            m_StudentRepository = studentRepository;
            m_UniversityRepository = universityRepository;
        }
        public void Handle(AddStudentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            const long universityId = 64805; //  long.Parse(ConfigFetcher.Fetch("StudentUnionToAddId"));
           var university = m_UniversityRepository.Load(universityId);

            var student = new Student(university, message.Id);

            m_StudentRepository.Save(student);
        }
    }
}
