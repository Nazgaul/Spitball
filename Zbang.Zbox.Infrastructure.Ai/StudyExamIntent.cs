namespace Zbang.Zbox.Infrastructure.Ai
{
    public class StudyExamIntent : IIntent
    {
        public string SearchQuery { get; set; }
        public string UniversityName { get; set; }
        public string CourseName { get; set; }
        public long Time { get; set; }
    }
}