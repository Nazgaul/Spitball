﻿using System;
using System.Linq;
using System.Security.Claims;

namespace Cloudents.Core.Extension
{
    public static class ClaimsPrincipalExtensions
    {
        public const string ClaimCountry = "Country";

        public static string GetCountryClaim(this ClaimsPrincipal principal)
        {
            var country = principal.Claims.First(w => string.Equals(w.Type, ClaimCountry, StringComparison.OrdinalIgnoreCase)).Value;
            return country != "None" ? country : null;
        }

    }
}