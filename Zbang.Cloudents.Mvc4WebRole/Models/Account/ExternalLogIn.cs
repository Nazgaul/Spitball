using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class ExternalLogIn
    {
        [Required]
        public string Token { get; set; }
        //public long? UniversityId { get; set; }
        public long? BoxId { get; set; }
    }
}