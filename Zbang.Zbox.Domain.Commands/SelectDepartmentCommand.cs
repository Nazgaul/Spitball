using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class SelectDepartmentCommand : ICommand
    {
        public SelectDepartmentCommand(long departmentId, long userId)
        {
            UserId = userId;
            DepartmentId = departmentId;
        }

        public long DepartmentId { get; private set; }
        public long UserId { get; private set; }
    }
}
