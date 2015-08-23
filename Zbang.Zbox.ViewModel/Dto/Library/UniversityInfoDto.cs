using System;
namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class UniversityInfoDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public long Id { get; set; }

        public string UniversityName { get; set; }

        public long BoxesCount { get; set; }
        public long ItemCount { get; set; }
        public long MemberCount { get; set; }

        public string WebSiteUrl { get; set; }
        public string MailAddress { get; set; }
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public long? TwitterWidgetId { get; set; }
        public string YouTubeUrl { get; set; }
        public string LetterUrl { get; set; }

    }
}
