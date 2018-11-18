using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;

namespace Cloudents.Core.Query
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
