﻿
namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserAccountDto
    {
        //profile
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Image { get; set; }
        public string University { get; set; }

        public long UniversityId { get; set; }

        //public string UniversityPic { get; set; }

        //account settings
        public string Email { get; set; }
        public string Language { get; set; }

        //storage
        public long AllocatedSize { get; set; }
        public long UsedSpace { get; set; }
    }
}
