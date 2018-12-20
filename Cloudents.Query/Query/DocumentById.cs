using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Query
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