using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Ico.Models;

namespace Cloudents.Ico.Controllers
{
    public class HomeController : Controller
    {
        private readonly Lazy<IServiceBusProvider> _serviceBus;

        public HomeController(Lazy<IServiceBusProvider> serviceBus)
        {
            _serviceBus = serviceBus;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("contact")]
        public async Task<IActionResult> ContactUs(ContactUs model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            await _serviceBus.Value.InsertMessageAsync(new ContactUsEmail(model.Name, model.Email, model.Text), token);
            return Ok();
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(Subscribe model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            await _serviceBus.Value.InsertMessageAsync(new ContactUsEmail(model.Email), token);
            return Ok();
        }

        //public IActionResult About()
        //{
        //    ViewData["Message"] = "Your application description page.";

        //    return View();
        //}

        //public IActionResult Contact()
        //{
        //    ViewData["Message"] = "Your contact page.";

        //    return View();
        //}

        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
