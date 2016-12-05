

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserWithStats
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
        public string UniversityName { get; set; }

        public string Url { get; set; }

        //public int NumClass { get; set; }
        public int NumItem { get; set; }
        public int NumFeed { get; set; }
        public int NumQuiz { get; set; }
        public int NumFriend { get; set; }
        public int NumBadge { get; set; }

        public bool Online { get; set; }
    }
}
