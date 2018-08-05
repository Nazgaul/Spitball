using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cloudents.Management.Pages
{
    [Authorize]
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


        [BindProperty] public string Model { get; set; }


        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var arr = Model.Split(new[] { "," ,Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);


            foreach (var t in arr)
            {
                if (long.TryParse(t, out var questionId))
                {
                    var command = new DeleteQuestionCommand(questionId);

                    await _commandBus.Value.DispatchAsync(command, token).ConfigureAwait(false);
                }
            }

            //return Ok();

            return RedirectToPage("/Index");
        }
    }
}