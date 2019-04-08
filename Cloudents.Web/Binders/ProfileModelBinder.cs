//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Models;
//using Cloudents.Query;
//using Cloudents.Query.Query;
//using Cloudents.Web.Extensions;
//using Cloudents.Web.Identity;
//using Cloudents.Web.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Newtonsoft.Json;

//namespace Cloudents.Web.Binders
//{
//    public class ProfileModelBinder : IModelBinder
//    {
//        private readonly IQueryBus _queryBus;
//        private readonly ICountryProvider _countryProvider;
//        private readonly UserManager<RegularUser> _userManager;

//        public ProfileModelBinder(IQueryBus queryBus, ICountryProvider countryProvider, UserManager<RegularUser> userManager)
//        {
//            _queryBus = queryBus;
//            _countryProvider = countryProvider;
//            _userManager = userManager;
//        }


//        public async Task BindModelAsync(ModelBindingContext bindingContext)
//        {
//            Guid? GetUniversityClaimValue()
//            {
//                var universityId = bindingContext.HttpContext.User.Claims.FirstOrDefault(f =>
//                    string.Equals(f.Type, AppClaimsPrincipalFactory.University, StringComparison.OrdinalIgnoreCase));
//                if (universityId?.Value != null && Guid.TryParse(universityId.Value, out var p))
//                {
//                    return p;
//                    //userQueryProfileDto.University.Id = p;
//                }
//                return null;
//            }

//            var token = bindingContext.HttpContext.RequestAborted;
//            var claim = bindingContext.ModelName;
//            if (claim == null)
//            {
//                bindingContext.Result = ModelBindingResult.Failed();
//                return;
//            }

//            var v = Enum.Parse<ProfileServiceQuery>(claim);
//            var profile = new UserProfile();


//            if (v == ProfileServiceQuery.None)
//            {
//                bindingContext.Result = ModelBindingResult.Success(profile);
//                return;
//            }

//            //if (v == ProfileServiceQuery.UniversityId)
//            //{
//            //    var uniId = GetUniversityClaimValue();
//            //    if (uniId.HasValue)
//            //    {
//            //        if (profile.University == null)
//            //        {
//            //            profile.University = new UserUniversityQueryProfileDto();
//            //        }
//            //        profile.University.Id = uniId.Value;
//            //    }

//            //    bindingContext.Result = ModelBindingResult.Success(profile);
//            //}
//            var countryTask = Task.FromResult<string>(null);
//            if (v.HasFlag(ProfileServiceQuery.Country))
//            {
//                countryTask =  _countryProvider.GetUserCountryAsync(token);
//            }

//            //if (v >= ProfileServiceQuery.University)
//            //{
//            //    if (bindingContext.HttpContext.User.Identity.IsAuthenticated)
//            //    {
//            //        var profileStr = bindingContext.HttpContext.User.Claims.FirstOrDefault(f =>
//            //            string.Equals(f.Type, AppClaimsPrincipalFactory.Profile, StringComparison.OrdinalIgnoreCase));
//            //        if (profileStr?.Value != null)
//            //        {
//            //            profile = JsonConvert.DeserializeObject<UserProfile>(profileStr.Value);
//            //        }

//            //        else
//            //        {
//            //            // var uniId = GetUniversityClaimValue();
//            //            var userId = _userManager.GetLongUserId(bindingContext.HttpContext.User);
//            //            var query = new UserDataQuery(userId);
//            //            profile = await _queryBus.QueryAsync(query, token);
//            //        }
//            //    }

//            //    //need to go to db
//            //}

//            profile.Country = await countryTask;
//            bindingContext.Result = ModelBindingResult.Success(profile);
//        }



//    }

//    [Flags]
//    public enum ProfileServiceQuery
//    {
//        None,
//       // UniversityId = 1,
//       // Country = 2,
//       // University = 4,
//        //Course = 8,
//        //Tag = 16,
//        //All = Tag | Course | University

//    }
//}