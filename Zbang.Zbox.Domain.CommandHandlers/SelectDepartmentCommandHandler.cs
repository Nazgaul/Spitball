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
    public class SelectDepartmentCommandHandler : ICommandHandler<SelectDepartmentCommand>
    {
        private readonly IRepository<Department> m_DepartmentRepository;
        private readonly IRepository<User> m_UserRepository;


        public SelectDepartmentCommandHandler(IRepository<Department> departmentRepository, IRepository<User> userRepository)
        {
            m_DepartmentRepository = departmentRepository;
            m_UserRepository = userRepository;
        }

        public void Handle(SelectDepartmentCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var department = m_DepartmentRepository.Load(message.DepartmentId);
            var user = m_UserRepository.Load(message.UserId);

            var uniId2 = department.University2 != null ? department.University2.Id : 0;

            if (user.University2.Id != department.University.Id
                && user.University2.Id != uniId2)
            {
                throw new UnauthorizedAccessException("cannot change to that department. its not in user university");
            }

            user.Department = department;
            m_UserRepository.Save(user);

        }
    }
}
