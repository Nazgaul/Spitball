
using System.ComponentModel.DataAnnotations;


namespace Zbang.Zbox.WebApi.Models
{
    public class AddComment
    {
        [Required]
        public string Comment { get; set; }

        public override string ToString()
        {
            return string.Format("  comment {0}",
                    Comment);

        }
    }
}