using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Share
{
    public class InviteSystemFromFacebook
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public bool Sex { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0} UserName: {1} FirstName: {2} MiddleNmae: {3} LastName: {4} Sex {5}", Id, UserName, FirstName, MiddleName, LastName, Sex);
        }
    }
}