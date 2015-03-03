using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    public class AccountController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }

        // GET api/Account
        [HttpGet]
        [Route("api/account/details")]

        public async Task<HttpResponseMessage> Details()
        {
            var retVal = await ZboxReadService.GetUserDataAsync(new GetUserDetailsQuery(User.GetCloudentsUserId()));
            return Request.CreateResponse(new
            {
                retVal.Id,
                retVal.UniversityId,
                retVal.Name,
                retVal.Image,
                retVal.IsAdmin,
                retVal.FirstTimeDashboard,
                retVal.Score,
                retVal.UniversityCountry,
                retVal.UniversityName,
            });
        }

    }
}
