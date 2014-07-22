namespace Zbang.Zbox.ViewModel.Dto.Library
{
    public class UniversityByPrefixDto
    {
        public UniversityByPrefixDto(string name, string image, long id, long memberCount)
        {
            Name = name;
            Image = image;
            Id = id;
            MemberCount = memberCount;
        }
        public string Name { get; private set; }
        public string Image { get; private set; }
        public long Id { get; private set; }

        //public bool NeedCode { get; private set; }
        public long MemberCount { get; private set; }

        //public string Country { get; set; }
    }

    public class UniversityLuceneDto
    {
        public string Image { get; private set; }
        public long MemberCount { get; private set; }

        public string Name { get; set; }

        public long Id { get; set; }
    }
}
