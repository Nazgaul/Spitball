using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class GetGeneralDepartmentCommand: ICommand
    {
        public GetGeneralDepartmentCommand(long userId, long universityId)
        {
            UserId = userId;
            UniversityId = universityId;
        }

        public long UniversityId { get; private set; }
        public long UserId { get; private set; }
    }
}
