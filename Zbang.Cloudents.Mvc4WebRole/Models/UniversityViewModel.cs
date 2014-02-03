using System.Collections.Generic;
using Zbang.Zbox.ViewModel.DTOs.UserDtos;
namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class UniversityViewModel
    {
        public UniversityViewModel()
        {
            Contributers = new List<UserDto>();
        }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool Exists { get; set; }

        public long BoxesCount { get; set; }
        public long ItemCount { get; set; }
        public long MemberCount { get; set; }

        public IEnumerable<UserDto> Contributers { get; set; }
    }
}