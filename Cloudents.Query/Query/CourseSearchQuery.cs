using System.Collections.Generic;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Query
{
    public class CourseSearchQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchQuery(string term)
        {
            Term = term;
        }

        public string Term { get; set; }
    }
}