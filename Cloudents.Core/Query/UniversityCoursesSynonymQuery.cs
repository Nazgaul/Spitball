using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Query
{
    public class UniversityCoursesSynonymQuery : IQuery<UniversityCoursesSynonymDto>
    {
        public UniversityCoursesSynonymQuery(long? universityId, IList<long> coursesIds)
        {
            UniversityId = universityId;
            CoursesIds = coursesIds;
        }

        [CanBeNull]
        public long? UniversityId { get;  }

        [CanBeNull]
        public IList<long> CoursesIds { get; private set; }
    }
}