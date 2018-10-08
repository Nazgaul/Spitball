using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Web.Models
{
    //TODO: should not be in this folder
    public class ClaimModelBinderAttribute : ModelBinderAttribute
    {
        public ClaimModelBinderAttribute(string claim/*, string errorResource*/)
        {
            //Claim = claim;
            //ErrorResource = errorResource;
            BinderType = typeof(ClaimModelBinder);
            Name = claim;
        }

       // public string Name { get; } => "Ram";
    }
}