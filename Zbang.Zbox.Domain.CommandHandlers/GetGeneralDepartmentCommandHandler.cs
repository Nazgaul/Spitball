using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class GetGeneralDepartmentCommandHandler : ICommandHandler<GetGeneralDepartmentCommand, GetGeneralDepartmentCommandResult>
    {
        private readonly IRepository<Library> m_Department;
        private readonly IRepository<User> m_User;
        private readonly IGuidIdGenerator m_IdGenerator;

        public GetGeneralDepartmentCommandHandler(IRepository<Library> department, IRepository<User> user, IGuidIdGenerator idGenerator)
        {
            m_Department = department;
            m_IdGenerator = idGenerator;
            m_User = user;
        }
        public GetGeneralDepartmentCommandResult Execute(GetGeneralDepartmentCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
           
            var user = m_User.Load(command.UserId);
            var university = user.University;
            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault Nhibernate doesnt support
            var departmentId = m_Department.Query()
                .Where(
                    w =>
                        w.University == university && w.Name != null &&
                        w.Name.Equals("general", StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault()?.Id;
                
            var newId = m_IdGenerator.GetId();
            if (departmentId != null) return new GetGeneralDepartmentCommandResult(departmentId.Value);
            var dep = new Library(newId, "General", university, user);
            m_Department.Save(dep);
            departmentId = newId;
            return new GetGeneralDepartmentCommandResult(departmentId.Value);
        }
    }
}
