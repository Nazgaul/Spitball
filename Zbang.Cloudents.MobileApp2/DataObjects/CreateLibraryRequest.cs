
using System.ComponentModel.DataAnnotations;


namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class CreateLibraryRequest
    {
        [Required]
        public string Name { get; set; }


        public string ParentId { get; set; }
    }
}