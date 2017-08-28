using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class SeoDto
    {
        public long BoxId { get; set; }
        public string BoxName { get; set; }
        public string Part { get; set; }

        public long? Id { get; set; }

        public string Name { get; set; }

        public string UniversityName { get; set; }
    }

    public class SitemapDto
    {
        public SitemapDto(SeoType type, int count)
        {
            Type = type;
            Count = count;
        }

        //public int BoxCount { get; set; }
        //public int ItemCount { get; set; }
        //public int QuizCount { get; set; }
        //public int FlashcardCount { get; set; }
        public SeoType Type { get;private set; }
        public int Count { get; private set; }
    }
}
