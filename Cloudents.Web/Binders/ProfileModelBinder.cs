using Cloudents.Core.Entities;
using Cloudents.Core.Models;
using Cloudents.Query;
using Cloudents.Query.Users;
using Cloudents.Web.Extensions;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;
using AppClaimsPrincipalFactory = Cloudents.Web.Identity.AppClaimsPrincipalFactory;

namespace Cloudents.Web.Binders
{
    public class ProfileModelBinder : IModelBinder
    {
        private readonly IQueryBus _queryBus;
        private readonly ICountryService _countryProvider;
        private readonly UserManager<User> _userManager;

        public ProfileModelBinder(IQueryBus queryBus, ICountryService countryProvider, UserManager<User> userManager)
        {
            _queryBus = queryBus;
            _countryProvider = countryProvider;
            _userManager = userManager;
        }


        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var token = bindingContext.HttpContext.RequestAborted;
            var claim = bindingContext.ModelName;
            if (claim == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            var profile = new UserProfile();
            var val = Enum.Parse<ProfileServiceQuery>(claim);

            var typeValues = Enum.GetValues(typeof(ProfileServiceQuery));
            foreach (ProfileServiceQuery value in typeValues)
            {
                if ((val & value) != value) continue;
                switch (value)
                {
                    case ProfileServiceQuery.None:
                        break;
                    case ProfileServiceQuery.UniversityId:
                        var universityId = bindingContext.HttpContext.User.Claims.FirstOrDefault(f =>
                            string.Equals(f.Type, AppClaimsPrincipalFactory.University, StringComparison.OrdinalIgnoreCase));
                        if (universityId?.Value != null && Guid.TryParse(universityId.Value, out var p))
                        {
                            profile.UniversityId = p;
                        }
                        break;
                    case ProfileServiceQuery.Country:
                        profile.Country = await _countryProvider.GetUserCountryAsync(token);
                        break;
                    case ProfileServiceQuery.Course:
                        if (bindingContext.HttpContext.User.Identity.IsAuthenticated)
                        {
                            var userId = _userManager.GetLongUserId(bindingContext.HttpContext.User);
                            var queryCourses = new UserCoursesQuery(userId);
                            var resultCourses = await _queryBus.QueryAsync(queryCourses, token);
                            profile.Courses = resultCourses.Select(s => s.Name);
                        }

                        break;
                       // throw new ArgumentOutOfRangeException();
                }
            }
            bindingContext.Result = ModelBindingResult.Success(profile);
        }
    }

    [Flags]
    public enum ProfileServiceQuery
    {
        None,
        UniversityId = 1,
        Country = 2,
        Course = 4,
    }
}