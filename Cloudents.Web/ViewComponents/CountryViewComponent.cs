﻿using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Web.Services;

namespace Cloudents.Web.ViewComponents
{
    [ViewComponent(Name = "Country")]
    public class CountryViewComponent : ViewComponent
    {
        private readonly ICountryProvider _countryProvider;

        public CountryViewComponent(ICountryProvider countryProvider)
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
