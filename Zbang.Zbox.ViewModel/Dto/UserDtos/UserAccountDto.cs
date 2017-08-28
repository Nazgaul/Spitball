namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserAccountDto
    {
        //profile
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Image { get; set; }
        public string University { get; set; }

        //public long UniversityId { get; set; }

        public string UniversityImage { get; set; }

        //account settings
        public string Email { get; set; }
        public string Language { get; set; }

        public bool System { get; set; }
    }
}
