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



        public long Id { get; private set; }
        public long? UserId { get; private set; }
    }

    public class DocumentSeoById : IQuery<DocumentSeoDto>
    {
        public DocumentSeoById(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }
}