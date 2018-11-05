using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
