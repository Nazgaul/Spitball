namespace Zbang.Zbox.Infrastructure.Ai
{
    public class JoinGroupIntent : IIntent
    {
        public string SearchQuery { get; set; }
        public string UniversityName { get; set; }
        public string CourseName { get; set; }
        public long Time { get; set; }
    }
}