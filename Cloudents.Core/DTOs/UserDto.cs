namespace Cloudents.Core.DTOs
{
    public class UserDto
    {
        public UserDto(long id, string name, int score)
        {
            Id = id;
            Name = name;
            Score = score;
        }

        // ReSharper disable once MemberCanBeProtected.Global need that for mark answer as correct.
        public UserDto()
        {
            
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
    }

    public class UserProfileDto : UserDto
    {
      

        public string UniversityName { get; set; }
    }

    public class UserAccountDto : UserDto
    {
        

        public string Token { get; set; }
        public decimal Balance { get; set; }

        public string Email { get; set; }

        public bool UniversityExists { get; set; }
    }
}