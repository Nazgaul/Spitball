using System;
using Cloudents.Core.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Cloudents.Web.Extensions.Binders
{
    public class GeoPointModelBinder : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(GeoPoint))
            {
                return new BinderTypeModelBinder(typeof(GeoPointEntityBinder));
            }

            return null;
        }
    }
}