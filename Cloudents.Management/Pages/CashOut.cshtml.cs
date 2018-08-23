using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Query.Admin;
using System.Threading;

namespace Cloudents.Management.Pages
{
    public class CashOutModel : PageModel
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly IQueryBus _queryBus;


        public CashOutModel(Lazy<ICommandBus> commandBus, IQueryBus queryBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
        }


        public async Task OnGet(CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            CashOuts = await _queryBus.QueryAsync<IEnumerable<CashOutDto>>(query, token);
        }

        [ViewData]
        public IEnumerable<CashOutDto> CashOuts { get; set; }
        
    }
}