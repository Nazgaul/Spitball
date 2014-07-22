namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class UniversityInfoDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public long Id { get; set; }
       // public string Uid { get; set; }

        public long BoxesCount { get; private set; }
        public long ItemCount { get; private set; }
        public long MemberCount { get; private set; }

        public string WebSiteUrl { get; private set; }
        public string MailAddress { get; private set; }
        public string FacebookUrl { get; private set; }
        public string TwitterUrl { get; private set; }
        public long? TwitterWidgetId { get; private set; }
        public string YouTubeUrl { get; private set; }
        public string LetterUrl { get; private set; }

        //public IEnumerable<UserDtos.UserDto> Contributers { get; set; }
    }
}
