// using System;
// using Microsoft.AspNetCore.Mvc;

// namespace Cloudents.Web.Controllers
// {
//     [ApiExplorerSettings(IgnoreApi = true)]

//     public class StudyRoomController : Controller
//     {
//         // GET
//         [Route("StudyRoom/{id?}")]
//         public IActionResult Index(Guid? id)
//         {
//             if (id.HasValue)
//             {
//                 return  RedirectToRoute("StudyRoomSettings", new {id=id.Value});
//             }
//             return View();
//         }


//         [Route("StudyRoomSettings/{id:guid}", Name = "StudyRoomSettings")]
//         public IActionResult Index(Guid id)
//         {
//             return View();
//         }
//     }
// }