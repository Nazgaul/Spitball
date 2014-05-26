using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.Library
{
    public class UniversityByFriendDto
    {
        public string Name { get; set; }
        public string Image { get; set; }

        public long Id { get; set; }

        public IEnumerable<FriendPerUniversityDto> Friends { get; set; }
    }
}
