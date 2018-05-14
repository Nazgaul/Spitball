using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.ModelBinding;

namespace Cloudents.Web.Models
{
    public class QuestionRequest
    {
        [Required]
        public long SubjectId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; }

        [Required]
        [Range(0.1, 1000)]
        public decimal Price { get; private set; }

        [BindNever]
        public long UserId { get; set; }
    }
}
