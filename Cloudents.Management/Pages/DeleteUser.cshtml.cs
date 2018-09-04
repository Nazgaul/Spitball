using System;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Cloudents.Management.Pages
{
    public class DeleteUserModel : PageModel
    {

        private readonly Lazy<ICommandBus> _commandBus;

        public DeleteUserModel(Lazy<ICommandBus> commandBus)
        {
            _commandBus = commandBus;
        }

        [BindProperty] public DeleteUser Model { get; set; }

        public class DeleteUser
        {
            [Required]
            public string Email { get; set; }
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            var command = new DeleteUserCommand
            {
                Email = Model.Email
            };

            await _commandBus.Value.DispatchAsync(command, token);
            return RedirectToPage("DeleteUser");
        }
    }
}