namespace Zbang.Zbox.Infrastructure.Ai
{
    public class HomeWorkIntent : IIntent
    {
        public string UniversityName { get; set; }
        public string CourseName { get; set; }

        public string SearchQuery { get; set; }
        public long Time { get; set; }
    }
}