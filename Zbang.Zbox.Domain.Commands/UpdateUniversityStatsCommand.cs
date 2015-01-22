using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateUniversityStatsCommand : ICommand
    {
        public UpdateUniversityStatsCommand(IEnumerable<long> universitiesIds)
        {
            UniversitiesIds = universitiesIds;
        }

        public IEnumerable<long> UniversitiesIds { get; private set; }
    }
}
