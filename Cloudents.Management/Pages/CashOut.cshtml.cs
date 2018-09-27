using System.Collections.Generic;
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
        private readonly IQueryBus _queryBus;


        public CashOutModel(IQueryBus queryBus)
        {
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