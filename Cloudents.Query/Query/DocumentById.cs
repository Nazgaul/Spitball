using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class DocumentById : IQuery<DocumentDetailDto>
    {
        public DocumentById(long id, long? userId)
        {
            Id = id;
            UserId = userId;
        }



        public long Id { get; }
        public long? UserId { get; }
    }

    public class DocumentSeoById : IQuery<DocumentSeoDto>
    {
        public DocumentSeoById(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class DocumentSeoByOldId : IQuery<DocumentSeoDto>
    {
        public DocumentSeoByOldId(long oldId)
        {
            OldId = oldId;
        }

        public long OldId { get; }
    }
}