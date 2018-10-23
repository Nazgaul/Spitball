using System;
using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Query
{
    public class UniversityCoursesSynonymQuery : IQuery<UniversityCoursesSynonymDto>
    {
        public UniversityCoursesSynonymQuery(Guid? universityId, IList<string> courses)
        {
            UniversityId = universityId;
            Courses = courses;
        }

        [CanBeNull]
        public Guid? UniversityId { get;  }

        [CanBeNull]
        public IList<string> Courses { get; private set; }
    }
}