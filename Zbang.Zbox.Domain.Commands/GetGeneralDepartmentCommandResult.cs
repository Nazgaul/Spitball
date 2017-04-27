using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class GetGeneralDepartmentCommandResult : ICommandResult
    {
        public Guid DepartmentId { get; private set; }

        public GetGeneralDepartmentCommandResult(Guid departmentId)
        {
            this.DepartmentId = departmentId;
        }
    }
}
