

namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
    public class LogInUserDto : UserDetailDto
    {
        public string Culture { get; set; }

        public long? UniversityId { get; set; }
        public long? UniversityWrapperId { get; set; }
    }
}
