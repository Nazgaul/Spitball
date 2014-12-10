using System;
using System.ComponentModel.DataAnnotations;


namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class MarkAnswer
    {
        [Required]
        public Guid? AnswerId { get; set; }


    }
}