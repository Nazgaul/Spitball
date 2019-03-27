//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Cloudents.Core.Storage;
//using Microsoft.AspNetCore.Mvc.ModelBinding;

//namespace Cloudents.Web.Binders
//{
//    public class StorageContainerBinder : IModelBinder
//    {
//        public  Task BindModelAsync(ModelBindingContext bindingContext)
//        {
//            string valueStr =
//                bindingContext.ActionContext.RouteData.Values[bindingContext.FieldName]?
//                    .ToString().ToLowerInvariant();


//            var fields = StorageContainer.GetAllValues();

//            var t =  fields.FirstOrDefault(f => string.Equals(f.Name, valueStr, StringComparison.OrdinalIgnoreCase));
//            if (t != null)
//            {
                
//                bindingContext.Result = ModelBindingResult.Success(t.GetValue(null));

//            }
//            else
//            {
//                ModelBindingResult.Failed();
//            }
//            return Task.CompletedTask;

//        }
//    }
//}