
using System;

namespace Zbang.Zbox.ViewModel.Dto
{
    [Serializable]
    public class LeaderBoardDto
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public string Url { get; set; }
      
    }
}
