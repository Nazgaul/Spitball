using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class DocumentById : IQuery<DocumentSeoDto>, IQuery<DocumentDetailDto>
    {
        public DocumentById(long id, long? userId = null)
        {
            Id = id;
            UserId = userId;
        }

    

        public long Id { get; private set; }
        public long? UserId { get; private set; }
    }
}