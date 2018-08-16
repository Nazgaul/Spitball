using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.DTOs.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Cloudents.Core.Enum;
using Cloudents.Core.Query.Admin;

namespace Cloudents.Management.Pages
{
    public class SendTokensModel : PageModel
    {
    
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;


        public SendTokensModel(Lazy<ICommandBus> commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }


        [BindProperty] public SendToken Model { get; set; }

        public class SendToken
        {
            [Required]
            public int UserId { get; set; }
            [Required]
            public int Price { get; set; }
            [Required]
            public int TypeId { get; set; }
        }

        [ViewData]
        public IEnumerable<string> Types { get; set; }

        public async Task OnGet(CancellationToken token)
        {
            // var query = new TransactionTypeQuery();
            //Types = await _queryBus.QueryAsync(query, token).ConfigureAwait(false);
       
             Types = Enum.GetNames(typeof(TransactionType));
            //return Ok(result);
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            var command = new CreateSendTokensCommand
            {
               UserId = Model.UserId,
               Price = Model.Price,
               TypeId = Model.TypeId
            };

            await _commandBus.Value.DispatchAsync(command, token);
            return RedirectToPage("SendTokens");
        }
    }
    
}