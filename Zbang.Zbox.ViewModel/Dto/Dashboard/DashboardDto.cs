using System.Collections.Generic;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;

namespace Zbang.Zbox.ViewModel.Dto.Dashboard
{
    public class DashboardDto
    {
        public UniversityDashboardInfoDto Info { get; set; }

        public IEnumerable<RecommendBoxDto> Recommended { get; set; }

        public IEnumerable<LeaderBoardDto> LeaderBoard { get; set; }

    }
}
