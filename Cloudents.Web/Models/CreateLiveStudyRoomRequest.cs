using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

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