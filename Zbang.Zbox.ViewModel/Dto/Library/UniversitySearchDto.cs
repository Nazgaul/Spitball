using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class UniversitySearchDto
    {
        public string Image { get; set; }

        public string Name { get; set; }

        public long Id { get; set; }

        public string Extra { get; set; }

        public string Country { get; set; }

        public int NoOfUsers { get; set; }

        public IEnumerable<string> UsersImages { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class UserImagesForUniversitySearchDto
    {
        public long UniversityId { get; set; }
        public string Image { get; set; }
    }

    public class UniversityToUpdateSearchDto
    {
        public IEnumerable<UniversitySearchDto> UniversitiesToUpdate { get; set; }

        public IEnumerable<long> UniversitiesToDelete { get; set; }
    }
}