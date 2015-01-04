using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Dashboard
{
    [Serializable]
    public class DashboardDto
    {
        public UniversityDashboardInfoDto Info { get; set; }


        public IEnumerable<LeaderBoardDto> LeaderBoard { get; set; }

    }
}
