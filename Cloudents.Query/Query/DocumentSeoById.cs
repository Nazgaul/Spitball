using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class DocumentSeoById : IQuery<DocumentSeoDto>
    {
        public DocumentSeoById(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}