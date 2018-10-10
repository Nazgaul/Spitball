using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class DocumentDataSeoById : IQuery<DocumentSeoDto>
    {
        public DocumentDataSeoById(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }
}