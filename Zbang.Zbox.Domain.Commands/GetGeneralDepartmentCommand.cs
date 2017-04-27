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
        public long UniversityId { get; set; }
        public long UserId { get; set; }
    }
}
