using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cloudents.Web.Filters
{
    //[AttributeUsage(AttributeTargets.Property)]
    //public sealed class ArrayMaxSizeAttribute : ValidationAttribute
    //{
    //    private readonly int _maxSize;

    //    public ArrayMaxSizeAttribute(int maxSize)
    //    {
    //        _maxSize = maxSize;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        if (value is IEnumerable<string> t)
    //        {
    //            if (t.Count() <= _maxSize)
    //            {
    //                return ValidationResult.Success;
    //            }
    //        }
    //        return new ValidationResult(GetErrorMessage());
    //    }

    //    private string GetErrorMessage()
    //    {
    //        return $"Maximum {_maxSize} files";
    //    }

    //}
}
