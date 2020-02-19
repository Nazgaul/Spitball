using Cloudents.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.ViewComponents
{
    [ViewComponent(Name = "Country")]
    public class CountryViewComponent : ViewComponent
    {
        private readonly ICountryService _countryProvider;

        public CountryViewComponent(ICountryService countryProvider)
        {
            _countryProvider = countryProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync(CancellationToken token)
        {
            var country = await _countryProvider.GetUserCountryAsync(token);
            return View(default, country ?? "us");
        }
    }
}
