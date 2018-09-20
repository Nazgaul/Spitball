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
    public class SuspendUserModel : PageModel
    {
        private readonly Lazy<ICommandBus> _commandBus;

        public SuspendUserModel(Lazy<ICommandBus> commandBus)
        {
            _commandBus = commandBus;
        }

        [BindProperty] public SuspendUser Model { get; set; }

        public class SuspendUser
        {
            [Required]
            public long Id { get; set; }
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            var command = new SuspendUserCommand(Model.Id);
            await _commandBus.Value.DispatchAsync(command, token);
            return RedirectToPage("DeleteUser");
        }
    }
}