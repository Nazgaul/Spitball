using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
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