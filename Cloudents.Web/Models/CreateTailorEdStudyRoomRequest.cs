using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateTailorEdStudyRoomRequest
    {
        public string Name { get; set; }

        [Range(1,50)]
        public int AmountOfUsers { get; set; }
    }

    public class EnterTailorEdRoomRequest
    {
        [Required] public string Code { get; set; }
    }

}