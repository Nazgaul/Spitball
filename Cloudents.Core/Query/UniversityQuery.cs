using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Query
{
    public class UniversityQuery : IQuery<IEnumerable<UniversityDto>>
    {
        public UniversityQuery(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}
