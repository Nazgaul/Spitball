using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class UpdateQuiz
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}