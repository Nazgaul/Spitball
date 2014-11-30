using System;


namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{
    [Serializable]
    public class RecommendBoxDto
    {
        public string Name { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }
        public string Picture { get; set; }
        public int MembersCount { get; set; }
        public int ItemCount { get; set; }
        public string Url { get; set; }
      

    }
}
