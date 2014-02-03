﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class TrimModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext, 
            ModelBindingContext bindingContext,
            System.ComponentModel.PropertyDescriptor propertyDescriptor,
            object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                var stringValue = (string)value;
                if (!string.IsNullOrEmpty(stringValue))
                    stringValue = stringValue.Trim();

                value = stringValue;
            }

            base.SetProperty(controllerContext, bindingContext,
                                propertyDescriptor, value);
        }

    }
}