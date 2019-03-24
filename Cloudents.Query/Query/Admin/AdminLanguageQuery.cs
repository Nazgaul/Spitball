using Cloudents.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Query.Query.Admin
{
    public class AdminLanguageQuery : 
            IQuery<IList<PendingCoursesDto>>, 
            IQuery<IList<PendingUniversitiesDto>>
    {
        public AdminLanguageQuery(string language)
        {
            Language = language;
        }
        public string Language { get; set; }
    }
}
