
namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
    public class UserAccountDto
    {
        //profile
        public string Name { get; set; }
        public string Image { get; set; }
        public string University { get; set; }

        public string UniversityPic { get; set; }

        //account settings
        public string Email { get; set; }
        public string Language { get; set; }

        //storage
        public long AllocatedSize { get; set; }
        public long UsedSpace { get; set; }
    }
}
