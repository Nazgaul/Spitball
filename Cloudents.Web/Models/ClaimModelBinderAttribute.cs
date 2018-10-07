using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    //TODO: should not be here
    public class ClaimModelBinderAttribute : ModelBinderAttribute
    {
        public ClaimModelBinderAttribute(string claim)
        {
            Claim = claim;
            BinderType = typeof(ClaimModelBinder);
        }

        public string Claim { get; }
     
    }
}