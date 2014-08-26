using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class SolveSheet
    {
        public SolveSheet(long timeTaken, int score)
        {
            TimeTaken = TimeSpan.FromTicks(timeTaken);
            Score = score;
        }
        public TimeSpan TimeTaken { get; set; }
        public int Score { get; set; }
        public IEnumerable<SolveQuestion> Questions { get; set; }
    }


}