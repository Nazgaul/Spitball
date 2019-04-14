using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StudyRoomController : Controller
    {

        public const string CookieName = "studyRoomId";
        // GET: /<controller>/
        [Route("studyRoom/{id:Guid}")]
        public IActionResult Index(Guid id)
        {
            //TODO: need to check room
            Response.Cookies.Append(CookieName, id.ToString(),new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                
            });
            return View();
        }
    }
}
