using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

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

            //if (context.Metadata.ModelType == typeof(GeographicCoordinate))
            //{
            //    return new BinderTypeModelBinder(typeof(GeoPointEntityBinder));
            //}

            //if (context.Metadata.ModelType == typeof(LocationQuery))
            //{
            //    return new BinderTypeModelBinder(typeof(LocationEntityBinder));
            //}

            //if (context.Metadata.ModelType == typeof(StorageContainer))
            //{
            //    return new BinderTypeModelBinder(typeof(StorageContainerBinder));
            //}
            //Note we cannot check for regular enum in here because we are checking for nullable enum
            if (Nullable.GetUnderlyingType(context.Metadata.ModelType)?.IsEnum == true)
            {
                return new BinderTypeModelBinder(typeof(NullableEnumEntityBinder));
            }

            if (context.Metadata.ModelType == typeof(DateTime) ||
                context.Metadata.ModelType == typeof(DateTime?))
            {
                return new BinderTypeModelBinder(typeof(DateTimeModelBinder));

            }

            //if (context.Metadata.ModelType == typeof(string))
            //{
            //this cause problem
            //return new BinderTypeModelBinder(typeof(HtmlEncodeModelBinder));
            //}
            return null;
        }
    }
}