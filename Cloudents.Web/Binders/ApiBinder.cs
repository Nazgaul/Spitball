using System;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Cloudents.Web.Binders
{
    public class ApiBinder : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(GeographicCoordinate))
            {
                
                return new BinderTypeModelBinder(typeof(GeoPointEntityBinder));
            }

            if (context.Metadata.ModelType == typeof(LocationQuery))
            {

                return new BinderTypeModelBinder(typeof(LocationEntityBinder));
            }

            if (Nullable.GetUnderlyingType(context.Metadata.ModelType)?.IsEnum == true)
            {
                return new BinderTypeModelBinder(typeof(NullableEnumEntityBinder));
            }

            return null;
        }
    }
}