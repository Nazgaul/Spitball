using Cloudents.Core.Command;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Management.Pages
{
    public class SendTokensModel : PageModel
    {

        private readonly Lazy<ICommandBus> _commandBus;


        public SendTokensModel(Lazy<ICommandBus> commandBus)
        {
            _commandBus = commandBus;
        }


        [BindProperty] public SendToken Model { get; set; }

        public class SendToken
        {
            [Required]
            public long UserId { get; set; }
            [Required]
            [Display(Name = "SBL")]
            public decimal Tokens { get; set; }
            [Required]
            public TransactionType TypeId { get; set; }

        }



        public void OnGet(CancellationToken token)
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            var command = new DistributeTokensCommand(Model.UserId, Model.Tokens, ActionType.None, Model.TypeId);
            await _commandBus.Value.DispatchAsync(command, token);
            return RedirectToPage("SendTokens");

        }
    }

}