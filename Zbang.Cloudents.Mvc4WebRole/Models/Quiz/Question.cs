using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class Question
    {
        //[Required]
        //public long QuizId { get; set; }
        [AllowHtml]
        public string Text { get; set; }

        [Required]
        public int CorrectAnswer { get; set; }
        [Required]
        public IEnumerable<Answer> Answers { get; set; }

        //[BindNever]
        public Guid? Id { get; set; }
    }
}