using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class TitleController : ApiController
    {
        private readonly ITitleSearch m_TitleSearch;

        public TitleController(ITitleSearch titleSearch)
        {
            m_TitleSearch = titleSearch;
        }

        public async Task<HttpResponseMessage> Get(string term, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));
            var result = await m_TitleSearch.SearchAsync(term, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
