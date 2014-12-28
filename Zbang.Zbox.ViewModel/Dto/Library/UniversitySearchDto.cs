using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class UniversitySearchDto
    {
        public string Image { get; set; }

        public string Name { get; set; }

        public long Id { get; set; }

        public string Extra { get; set; }
    }

    public class UniversityToUpdateSearchDto
    {
        public IEnumerable<UniversitySearchDto> UniversitiesToUpdate { get; set; }

        public IEnumerable<long> UniversitiesToDelete { get; set; }
    }
}