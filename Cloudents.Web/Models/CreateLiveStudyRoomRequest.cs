using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateLiveStudyRoomRequest 
    {
        [Required]
        public string Name { get; set; }

       
        [Required]
        public DateTime Date { get; set; }
      
    }
}