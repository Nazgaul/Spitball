using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloudents.Management.Pages
{
    public class MarkAsCorrectModel : PageModel
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;

        public MarkAsCorrectModel(Lazy<ICommandBus> commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        public async Task OnGet(CancellationToken token)
        {
            var query = new FictiveUsersQuestionsWithoutCorrectAnswerQuery();
            Questions = await _queryBus.QueryAsync(query, token);
        }

        [ViewData]
        public IEnumerable<QuestionWithoutCorrectAnswerDto> Questions { get; set; }

        [BindProperty] public QuestionAnswer Model  { get; set; }

        public class QuestionAnswer
        {
            [ Required]
            public long? QuestionId { get; set; }

            [BindProperty, Required]
            public Guid? AnswerId { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Debug.Assert(Model.AnswerId != null, "Model.AnswerId != null");
            Debug.Assert(Model.QuestionId != null, "Model.QuestionId != null");
            var command = new MarkAnswerAsCorrectCommand(Model.AnswerId.Value, Model.QuestionId.Value);

            await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
            //return Ok();

            return RedirectToPage("MarkAsCorrect");
            //return RedirectToPage("/Index");
        }
     }
}