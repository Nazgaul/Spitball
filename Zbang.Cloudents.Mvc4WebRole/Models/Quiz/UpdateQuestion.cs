using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class UpdateQuestion
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public Guid Id { get; set; }

    }
}