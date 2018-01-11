using System.Linq;
using System.Web.Http.ModelBinding;

namespace Cloudents.Mobile.Extensions
{
    /// <summary>
    /// Extension for modelstate
    /// </summary>
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Return the first error in the model state
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetError(this ModelStateDictionary model)
        {
            return model.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage)).FirstOrDefault();
        }
    }
}