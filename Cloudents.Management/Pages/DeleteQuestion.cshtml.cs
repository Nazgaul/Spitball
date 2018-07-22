using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Management.Command;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloudents.Management.Pages
{
    public class DeleteQuestionModel : PageModel
    {
        private readonly Lazy<ICommandBus> _commandBus;

        public DeleteQuestionModel(Lazy<ICommandBus> commandBus)
        {
            _commandBus = commandBus;
        }

        public void OnGet()
        {

        }


        [BindProperty] public DeleteQuestionCommand Model { get; set; }


        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //var command = new MarkAnswerAsCorrectCommand(Model.AnswerId.Value, Model.QuestionId.Value);

            await _commandBus.Value.DispatchAsync(Model, token).ConfigureAwait(false);
            //return Ok();

            return RedirectToPage("/Index");
        }
    }
}