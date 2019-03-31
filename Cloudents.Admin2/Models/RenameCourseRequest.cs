using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class RenameCourseRequest
    {
        public RenameCourseRequest(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
        public string OldName { get; set; }
        [StringLength(150, MinimumLength = 4, ErrorMessage = "StringLength")]
        public string NewName { get; set; }
    }

    public class RenameUniversityRequest
    {
        public RenameUniversityRequest(Guid universityId, string newName)
        {
            UniversityId = universityId;
            NewName = newName;
        }
        public Guid UniversityId { get; set; }
        [StringLength(150, MinimumLength = 4, ErrorMessage = "StringLength")]
        public string NewName { get; set; }
    }
}
