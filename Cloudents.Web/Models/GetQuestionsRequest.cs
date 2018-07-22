namespace Cloudents.Web.Models
{
    public class GetQuestionsRequest : IPaging
    {
        public string[] Term { get; set; }
        public int? Page { get; set; }
    }
}