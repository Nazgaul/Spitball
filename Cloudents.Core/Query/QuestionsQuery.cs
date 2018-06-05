namespace Cloudents.Core.Query
{
    public class QuestionsQuery
    {
        public string Term { get; set; }
        public string[] Source { get; set; }
        public int Page { get; set; }
    }
}