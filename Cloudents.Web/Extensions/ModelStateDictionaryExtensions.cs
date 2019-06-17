using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Web.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddIdentityModelError(this ModelStateDictionary model, IdentityResult result)
        {
            AddIdentityModelError(model, string.Empty, result);
        }

        public static void AddIdentityModelError(this ModelStateDictionary model, string key, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                model.AddModelError(key, error.Description);
            }
        }

    }
}