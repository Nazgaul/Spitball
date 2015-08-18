using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class UpdateUniversityRequest
    {
        [Required]
        public long UniversityId { get; set; }

        [Obsolete]
        public string Code { get; set; }

        public long? DepartmentId { get; set; }

        public string GroupNumber { get; set; }
        public string RegisterNumber { get; set; }

        public string StudentId { get; set; }

        //public override string ToString()
        //{
        //    return string.Format(
        //        "University id {0} code {1} DepartmentId {2} GroupNumber {3} RegisterNumber {4} studentID {5}",
        //        UniversityId, Code, DepartmentId, GroupNumber, RegisterNumber, StudentId);

        //}
    }
}