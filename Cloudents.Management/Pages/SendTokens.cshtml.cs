using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Cloudents.Core.Enum;

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
            public decimal SBL { get; set; }
            [Required]
            public TransactionType TypeId { get; set; }
        }



        public void OnGet(CancellationToken token)
        {     
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            var command = new GiveTokensCommand
            {
               UserId = Model.UserId,
               Price = Model.SBL,
               TypeId = Model.TypeId
            };

            await _commandBus.Value.DispatchAsync(command, token);
            return RedirectToPage("SendTokens");

        }
    }
    
}