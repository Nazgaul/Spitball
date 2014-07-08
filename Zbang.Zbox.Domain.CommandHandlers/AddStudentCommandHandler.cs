using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
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
            var university = m_UniversityRepository.Load(64805L);

            var student = new Student(university, message.Id);

            m_StudentRepository.Save(student);
        }
    }
}
