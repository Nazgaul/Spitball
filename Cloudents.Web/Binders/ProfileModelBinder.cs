using Cloudents.Core.Entities;
using Cloudents.Core.Models;
using Cloudents.Query;
using Cloudents.Query.Users;
using Cloudents.Web.Extensions;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

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
                    case ProfileServiceQuery.Country:
                        profile.Country = await _countryProvider.GetUserCountryAsync(token);
                        break;
                    case ProfileServiceQuery.Subscribers:
                        //case ProfileServiceQuery.Course:
                        if (bindingContext.HttpContext.User.Identity.IsAuthenticated)
                        {
                            var userId = _userManager.GetLongUserId(bindingContext.HttpContext.User);

                            var query = new UserSubscribersQuery(userId);
                            var result = await _queryBus.QueryAsync(query, token);
                            profile.Subscribers = result;
                            
                        }
                        break;
                        // throw new ArgumentOutOfRangeException();
                }
            }
            bindingContext.Result = ModelBindingResult.Success(profile);
        }
    }
}