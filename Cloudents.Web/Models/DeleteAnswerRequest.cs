using System;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    public class DeleteAnswerRequest
    {
        [FromRoute]
        public Guid Id { get; set; }
    }
}