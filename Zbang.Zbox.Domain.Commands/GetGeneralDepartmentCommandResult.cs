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
        public Guid? departmentId { get; set; }
        private long universityId;

        public GetGeneralDepartmentCommandResult(Guid? departmentId, long universityId)
        {
            this.departmentId = departmentId;
            this.universityId = universityId;
        }
    }
}
