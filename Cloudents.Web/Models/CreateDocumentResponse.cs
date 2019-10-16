namespace Cloudents.Web.Models
{
    public class CreateDocumentResponse
    {
        public CreateDocumentResponse( bool published)
        {
            Published = published;
        }


        public bool Published { get;private set; }
    }
}