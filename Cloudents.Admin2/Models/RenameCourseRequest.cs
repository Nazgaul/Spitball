using System.ComponentModel.DataAnnotations;

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

   
}
