using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class DocumentById : IQuery<DocumentSeoDto>, IQuery<DocumentDetailDto>
    {
        public DocumentById(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }
}