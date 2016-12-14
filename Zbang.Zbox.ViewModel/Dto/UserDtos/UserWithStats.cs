

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserWithStats
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }

        public string LevelName { get; set; }
        public int NextLevel { get; set; }

        public int NumItem { get; set; }
        public int NumFeed { get; set; }
        public int NumQuiz { get; set; }
        public int NumFriend { get; set; }
        public int NumFlashcard { get; set; }

    }
}
