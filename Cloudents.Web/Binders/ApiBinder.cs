using System;
using System.Linq;
using System.Reflection;
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

            //if (Nullable.GetUnderlyingType(context.Metadata.ModelType)?.IsEnum == true)
            if (context.Metadata.IsEnum)
            {
                return new BinderTypeModelBinder(typeof(NullableEnumEntityBinder));
            }

            //if (context.Metadata.ModelType == typeof(string))
            //{
                //this cause problem
                //return new BinderTypeModelBinder(typeof(HtmlEncodeModelBinder));
            //}
            return null;
        }
    }

    //public class ProtectedIdModelBinderProvider : IModelBinderProvider
    //{
    //    public IModelBinder GetBinder(ModelBinderProviderContext context)
    //    {
    //        if (context.Metadata.IsComplexType) return null;

    //        var propName = context.Metadata.PropertyName;
    //        if (propName == null) return null;

    //        var propInfo = context.Metadata.ContainerType.GetProperty(propName);
    //        if (propInfo == null) return null;

    //       // var t = propInfo.GetCustomAttributes<ProtectedIdAttribute>();
    //        var attribute = propInfo.GetCustomAttributes(
    //            typeof(ReturnUrlAttribute), false).FirstOrDefault();
    //        if (attribute == null) return null;

    //        return new BinderTypeModelBinder(typeof(ReturnUrlEntityBinder));
    //    }
    //}


    //public class ReturnUrlAttribute
    //    : Attribute
    //{ }
}