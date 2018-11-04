using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
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