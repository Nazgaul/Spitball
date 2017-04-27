using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class GetGeneralDepartmentCommandResult : ICommandResult
    {
        public Guid DepartmentId { get; private set; }

        public GetGeneralDepartmentCommandResult(Guid departmentId)
        {
            DepartmentId = departmentId;
        }
    }
}
