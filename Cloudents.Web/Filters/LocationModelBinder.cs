using System;
using Cloudents.Core.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Cloudents.Web.Filters
{
    public class LocationModelBinder : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Location))
            {
                return new BinderTypeModelBinder(typeof(LocationEntityBinder));
            }

            return null;
        }
    }
}