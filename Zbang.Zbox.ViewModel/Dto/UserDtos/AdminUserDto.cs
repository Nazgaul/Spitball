using System;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class AdminUserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Department { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
