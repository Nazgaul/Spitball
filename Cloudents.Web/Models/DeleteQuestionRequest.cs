﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class DeleteQuestionRequest
    {
        [FromRoute]
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
    }
}