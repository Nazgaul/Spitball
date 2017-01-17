namespace Zbang.Zbox.Infrastructure.Ai
{
    public abstract class BaseIntent : IIntent
    {
        [WitApiName("search_query")]
        public string Term { get; set; }

        [WitApiName("UniversityName")]

        public string University { get; set; }

        [WitApiName("CourseName")]
        public string Course { get; set; }
    }
}