using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class MigrateUniversityRequest
    {
        public Guid UniversityToRemove { get; set; }
        public Guid UniversityToKeep { get; set; }
    }
}
