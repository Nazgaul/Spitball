namespace Cloudents.Web.Models
{
    public class CreateDocumentResponse
    {
        public CreateDocumentResponse(string url, bool published)
        {
            Url = url;
            Published = published;
        }

        public string Url { get; set; }

        public bool Published { get;private set; }
    }
}