using Microsoft.AspNetCore.Mvc;
using System;

namespace Cloudents.Web.Models
{
    public class DeleteAnswerRequest
    {
        [FromRoute]
        public Guid Id { get; set; }
    }
}