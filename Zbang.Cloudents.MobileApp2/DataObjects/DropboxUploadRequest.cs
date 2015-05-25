using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class DropboxUploadRequest
    {
        [Required]
        public long BoxId { get; set; }
        [Required]
        public string FileUrl { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid? TabId { get; set; }
        public bool Question { get; set; }

       
    }
}