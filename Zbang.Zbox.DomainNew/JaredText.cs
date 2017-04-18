using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    class JaredText
    {
        public int Id { get; set; }
        [Required]
        public CategoryTextType Type { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
