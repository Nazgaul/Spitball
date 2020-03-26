using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    //TODO: should not be in this folder
    //public class ClaimModelBinderAttribute : ModelBinderAttribute
    //{
    //    public ClaimModelBinderAttribute(string claim)
    //    {
    //        BinderType = typeof(ClaimModelBinder);
    //        Name = claim;
    //    }
    //}

    public class ProfileModelBinderAttribute : ModelBinderAttribute
    {
        public ProfileModelBinderAttribute(ProfileServiceQuery claim)
        {
            BinderType = typeof(ProfileModelBinder);
            Name = claim.ToString("G");
        }
    }
}