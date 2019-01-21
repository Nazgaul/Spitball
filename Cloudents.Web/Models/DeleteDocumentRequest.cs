﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    public class DeleteDocumentRequest
    {
        [FromRoute]
        [Range(1, long.MaxValue, ErrorMessage = "Range")]
        public long Id { get; set; }
    }
}