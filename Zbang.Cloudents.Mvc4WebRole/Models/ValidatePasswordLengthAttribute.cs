using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        //private static string _defaultErrorMessage = ValidatePasswordResources.MustBeAtLeast;
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public override string FormatErrorMessage(string name)
        {
            ValidatePasswordResources.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var resource = ValidatePasswordResources.MustBeAtLeast;

            //Resources.ValidatePasswordResources.MustBeAtLeast
            return String.Format(ErrorMessageString,
                 _minCharacters);
        }

        public override bool IsValid(object value)
        {
            var valueAsString = value as string;
            if (string.IsNullOrEmpty(valueAsString))
                return true;
            return valueAsString.Length >= _minCharacters;

            // return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }


    }
}