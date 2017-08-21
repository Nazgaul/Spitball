using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class JaredText
    {
        public JaredText()
        {
        }

        public int JaredTextId { get; set; }
        [Required]
        public CategoryTextType Action { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
