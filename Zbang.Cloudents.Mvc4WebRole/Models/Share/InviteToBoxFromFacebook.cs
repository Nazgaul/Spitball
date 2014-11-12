using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Share
{
    public class InviteToBoxFromFacebook
    {
        [Required]
        public long Id { get; set; }
        
        public string UserName { get; set; }
        //[Required]
        //public string Name { get; set; }

        [Required]
        [Range(1,long.MaxValue)]
        public long BoxId { get; set; }


        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public bool Sex { get; set; }


    }
}