using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.Dashboard
{
    public class DashboardDto
    {
        public IEnumerable<UserDtos.UserDto> Friends { get; set; }
        public IEnumerable<BoxDto> Boxes { get; set; }

        //public IEnumerable<ActivityDtos.BaseActivityDto> Wall { get; set; }
        public IEnumerable<WallDto> Wall { get; set; }

        //public UniversityDashboardInfoDto Uni { get; set; }
    }
}
