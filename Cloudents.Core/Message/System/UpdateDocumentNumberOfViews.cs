namespace Cloudents.Core.Message.System
{
    public class UpdateDocumentNumberOfViews : ISystemQueueMessage
    {

        public UpdateDocumentNumberOfViews(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }


}