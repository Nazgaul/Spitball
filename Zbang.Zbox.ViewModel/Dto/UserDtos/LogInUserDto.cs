

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class LogInUserDto : UserDetailDto
    {
        public string Culture { get; set; }

        public long? UniversityId { get; set; }

        public long? UniversityData { get; set; }
    }
}
