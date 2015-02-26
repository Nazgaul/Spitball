using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    //[AuthorizeLevel(AuthorizationLevel.User)]
    public class BoxesController : ApiController
    {
        public ApiServices Services { get; set; }

        public IZboxCacheReadService ZboxReadService { get; set; }

        // GET api/Boxes
        public async Task<HttpResponseMessage> Get(int page)
        {
            var userid = User.GetCloudentsUserId();
            var query = new GetBoxesQuery(userid, page, 15);
            var data = await ZboxReadService.GetUserBoxes(query);

            return Request.CreateResponse(data.Select(s => new
            {
                s.Name,
                s.Id,
                s.ItemCount,
                s.CommentCount
            }));
        }

    }
}
