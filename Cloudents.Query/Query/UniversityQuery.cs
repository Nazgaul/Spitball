using System;
using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class UniversityQuery : IQuery<UniversityDto>
    {
        public UniversityQuery(Guid universityId)
        {
            UniversityId = universityId;
        }

        public Guid UniversityId { get; private set; }
    }
}
