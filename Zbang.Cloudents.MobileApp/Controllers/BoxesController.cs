using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    public class BoxesController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        public async Task<HttpResponseMessage> Get(int page)
        {
            var query = new GetBoxesQuery(1, page, 15);
            var data = await ZboxReadService.GetUserBoxes(query);

            return Request.CreateResponse(data.Select(s => new
              {
                  s.Name,
                  s.Url,
                  s.ItemCount,
                  s.CommentCount
              }));
        }

    }
}
