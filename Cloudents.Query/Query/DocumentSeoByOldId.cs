using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class DocumentSeoByOldId : IQuery<DocumentSeoDto>
    {
        public DocumentSeoByOldId(long oldId)
        {
            OldId = oldId;
        }

        public long OldId { get; }
    }
}