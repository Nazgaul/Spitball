using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
   public class GamificationBoardDto
    {
       public int Score { get; set; }
       public string Level { get; set; }
       public int NextLevel { get; set; }
       public int BadgeCount { get; set; }
       public int Number { get; set; }
    }

    public class LevelDto
    {
        public int Score { get; set; }
        //public string Level { get; set; }
        public int NextLevel { get; set; }
        public int Number { get; set; }
    }

    public class BadgeDto
    {
        public BadgeDto(BadgeType badge, int progress)
        {
            Badge = badge.GetEnumDescription();
            Progress = progress;
        }
        public string Badge { get; private set; }

        public int Progress { get;private set; }
    }

  
}
