using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class SolveSheet
    {
        public SolveSheet(long timeTaken, int score, Guid? id)
        {
            TimeTaken = TimeSpan.FromTicks(timeTaken);
            Score = score;
           // if (like != Guid.Empty)
           // {
                Like = id;
           // }
        }
        public TimeSpan TimeTaken { get; set; }
        public int Score { get; set; }
        public IEnumerable<SolveQuestion> Questions { get; set; }

        public Guid? Like { get; set; }
    }


}