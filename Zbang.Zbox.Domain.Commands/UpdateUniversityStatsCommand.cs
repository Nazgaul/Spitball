using System.Collections.Generic;
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
