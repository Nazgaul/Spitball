namespace Cloudents.Core.DTOs
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class UserProfileDto : UserDto
    {
        public string UniversityName { get; set; }
    }

    public class UserAccountDto : UserDto
    {
        public string Token { get; set; }
        public decimal Balance { get; set; }
        public decimal Dollar => Balance / 40;
    }
}