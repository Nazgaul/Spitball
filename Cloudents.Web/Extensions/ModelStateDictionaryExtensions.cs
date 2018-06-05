using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Web.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static void AddIdentityModelError(this ModelStateDictionary model, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                model.AddModelError(string.Empty, error.Description);
            }
        }
    }
}