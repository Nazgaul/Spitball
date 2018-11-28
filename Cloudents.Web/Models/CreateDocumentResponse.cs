namespace Cloudents.Web.Models
{
    public class CreateDocumentResponse
    {
        public CreateDocumentResponse(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}