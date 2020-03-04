using Cloudents.Core.DTOs.Documents;

namespace Cloudents.Web.Models
{
    public class DocumentPreviewResponse
    {
        public DocumentPreviewResponse(DocumentDetailDto details, object preview, string content)
        {
            Details = details;
            Content = content;
            Preview = preview;
        }

        public DocumentDetailDto Details { get; }
        public object Preview { get; }

        public string Content { get; }
    }



}