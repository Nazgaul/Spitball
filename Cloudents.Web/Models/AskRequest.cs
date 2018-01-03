namespace Cloudents.Web.Models
{
    public class AskRequest
    {
        public int? Page { get; set; }
        public string[] Term { get; set; }
    }
}