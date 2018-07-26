using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MarkAnswerAsCorrectCommand = Cloudents.Management.Command.MarkAnswerAsCorrectCommand;

namespace Cloudents.Management.Pages
{
   // [Authorize]
    public class MarkAsCorrectModel : PageModel
    {

        private readonly Lazy<ICommandBus> _commandBus;

        public MarkAsCorrectModel(Lazy<ICommandBus> commandBus)
        {
            _commandBus = commandBus;
        }

        public void OnGet()
        {
        }

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

            return RedirectToPage("/Index");
        }
     }
}