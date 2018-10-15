using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class CoursesQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CoursesQuery(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
    }
}