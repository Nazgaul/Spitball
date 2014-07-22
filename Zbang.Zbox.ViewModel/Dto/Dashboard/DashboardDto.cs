using System.Collections.Generic;
using Zbang.Zbox.ViewModel.DTOs.UserDtos;

namespace Zbang.Zbox.ViewModel.Dto.Dashboard
{
    public class DashboardDto
    {
        public IEnumerable<UserDto> Friends { get; set; }
        public IEnumerable<BoxDto> Boxes { get; set; }

        public IEnumerable<WallDto> Wall { get; set; }

    }
}
