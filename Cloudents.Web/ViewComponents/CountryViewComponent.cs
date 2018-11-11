using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.ViewComponents
{
    [ViewComponent(Name = "country")]
    public class CountryViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync([ModelBinder(typeof(CountryModelBinder))] string _country)
        {
            return View(default, _country ?? "us");
        }
    }
}
